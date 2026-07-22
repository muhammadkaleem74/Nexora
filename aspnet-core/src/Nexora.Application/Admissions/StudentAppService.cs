using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using Nexora.Admissions.Dto;
using Nexora.Authorization;
using Nexora.Domain.Admissions;
using System.Linq.Dynamic.Core;

namespace Nexora.Admissions;

[AbpAuthorize(PermissionNames.Pages_Admissions_Students)]
public class StudentAppService
    : AsyncCrudAppService<Student, StudentListDto, long,
                          PagedStudentRequestDto,
                          CreateStudentDto,
                          UpdateStudentDto>,
      IStudentAppService
{
    private readonly IRepository<EnrollmentHistory, long> _enrollmentHistoryRepository;
    private readonly IRepository<StudentGuardian, long> _studentGuardianRepository;
    private readonly GuardianManager _guardianManager;

    public StudentAppService(
        IRepository<Student, long> repository,
        IRepository<EnrollmentHistory, long> enrollmentHistoryRepository,
        IRepository<StudentGuardian, long> studentGuardianRepository,
        GuardianManager guardianManager)
        : base(repository)
    {
        _enrollmentHistoryRepository = enrollmentHistoryRepository;
        _studentGuardianRepository = studentGuardianRepository;
        _guardianManager = guardianManager;
    }

    [AbpAuthorize(PermissionNames.Pages_Admissions_Students_Create)]
    public override async Task<StudentListDto> CreateAsync(CreateStudentDto input)
    {
        CheckCreatePermission();

        var student = ObjectMapper.Map<Student>(input);
        await Repository.InsertAsync(student);
        await CurrentUnitOfWork.SaveChangesAsync();

        return MapToEntityDto(student);
    }

    [AbpAuthorize(PermissionNames.Pages_Admissions_Students_Edit)]
    public override async Task<StudentListDto> UpdateAsync(UpdateStudentDto input)
    {
        var student = await Repository.GetAsync(input.Id);
        ObjectMapper.Map(input, student);
        return MapToEntityDto(student);
    }

    [AbpAuthorize(PermissionNames.Pages_Admissions_Students_Delete)]
    public override async Task DeleteAsync(EntityDto<long> input)
    {
        await Repository.DeleteAsync(input.Id);
    }

    public async Task<StudentDetailDto> GetDetailAsync(long id)
    {
        var student = await Repository.GetAll()
            .Include(s => s.Campus)
            .Include(s => s.CurrentGradeLevel)
            .Include(s => s.CurrentSection)
            .Include(s => s.StudentGuardians).ThenInclude(sg => sg.Guardian)
            .Include(s => s.EnrollmentHistories).ThenInclude(eh => eh.AcademicYear)
            .Include(s => s.EnrollmentHistories).ThenInclude(eh => eh.GradeLevel)
            .Include(s => s.EnrollmentHistories).ThenInclude(eh => eh.Section)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (student == null)
            throw new UserFriendlyException("Student not found.");

        return ObjectMapper.Map<StudentDetailDto>(student);
    }

    public async Task<List<EnrollmentHistoryDto>> GetEnrollmentHistoryAsync(long studentId)
    {
        var histories = await _enrollmentHistoryRepository.GetAll()
            .Include(h => h.AcademicYear)
            .Include(h => h.GradeLevel)
            .Include(h => h.Section)
            .Where(h => h.StudentId == studentId)
            .OrderByDescending(h => h.AcademicYear.StartDate)
            .ToListAsync();

        return ObjectMapper.Map<List<EnrollmentHistoryDto>>(histories);
    }

    [AbpAuthorize(PermissionNames.Pages_Admissions_Students_ManageGuardians)]
    public async Task<StudentGuardianDto> AddGuardianAsync(long studentId, CreateGuardianDto input)
    {
        var student = await Repository.GetAsync(studentId);
        var guardian = await _guardianManager.FindOrCreateAsync(input);

        if (input.IsPrimaryContact)
            await _guardianManager.ClearPrimaryContactsAsync(
                _studentGuardianRepository,
                sg => sg.StudentId == studentId);

        var link = new StudentGuardian
        {
            StudentId = student.Id,
            GuardianId = guardian.Id,
            IsPrimaryContact = input.IsPrimaryContact,
            CanPickupStudent = input.IsPrimaryContact
        };

        await _studentGuardianRepository.InsertAsync(link);
        await CurrentUnitOfWork.SaveChangesAsync();

        return new StudentGuardianDto
        {
            Id = link.Id,
            GuardianId = guardian.Id,
            FullName = guardian.FullName,
            Relationship = guardian.Relationship,
            NationalIdNumber = guardian.NationalIdNumber,
            Email = guardian.Email,
            Phone = guardian.Phone,
            Occupation = guardian.Occupation,
            IsPrimaryContact = link.IsPrimaryContact,
            CanPickupStudent = link.CanPickupStudent
        };
    }

    [AbpAuthorize(PermissionNames.Pages_Admissions_Students_ManageGuardians)]
    public async Task RemoveGuardianAsync(long studentGuardianId)
    {
        await _studentGuardianRepository.DeleteAsync(studentGuardianId);
    }

    public async Task<List<StudentGuardianDto>> GetGuardiansAsync(long studentId)
    {
        var links = await _studentGuardianRepository.GetAll()
            .Include(sg => sg.Guardian)
            .Where(sg => sg.StudentId == studentId)
            .ToListAsync();

        return links.Select(sg => new StudentGuardianDto
        {
            Id = sg.Id,
            GuardianId = sg.GuardianId,
            FullName = sg.Guardian.FullName,
            Relationship = sg.Guardian.Relationship,
            NationalIdNumber = sg.Guardian.NationalIdNumber,
            Email = sg.Guardian.Email,
            Phone = sg.Guardian.Phone,
            Occupation = sg.Guardian.Occupation,
            IsPrimaryContact = sg.IsPrimaryContact,
            CanPickupStudent = sg.CanPickupStudent
        }).ToList();
    }

    protected override IQueryable<Student> CreateFilteredQuery(PagedStudentRequestDto input)
    {
        return Repository
            .GetAllIncluding(s => s.Campus, s => s.CurrentGradeLevel, s => s.CurrentSection)
            .WhereIf(!input.Keyword.IsNullOrWhiteSpace(),
                s => s.FirstName.Contains(input.Keyword) ||
                     s.LastName.Contains(input.Keyword) ||
                     s.RegistrationNumber.Contains(input.Keyword) ||
                     s.GRNumber.Contains(input.Keyword))
            .WhereIf(input.Status.HasValue, s => s.Status == input.Status.Value)
            .WhereIf(input.CampusId.HasValue, s => s.CampusId == input.CampusId.Value)
            .WhereIf(input.CurrentGradeLevelId.HasValue, s => s.CurrentGradeLevelId == input.CurrentGradeLevelId.Value)
            .WhereIf(input.CurrentSectionId.HasValue, s => s.CurrentSectionId == input.CurrentSectionId.Value);
    }

    protected override IQueryable<Student> ApplySorting(
        IQueryable<Student> query, PagedStudentRequestDto input)
    {
        if (input.Sorting.IsNullOrWhiteSpace())
            return query.OrderBy(s => s.FirstName).ThenBy(s => s.LastName);

        return query.OrderBy(input.Sorting);
    }
}
