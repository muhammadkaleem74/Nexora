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

    public StudentAppService(
        IRepository<Student, long> repository,
        IRepository<EnrollmentHistory, long> enrollmentHistoryRepository)
        : base(repository)
    {
        _enrollmentHistoryRepository = enrollmentHistoryRepository;
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
            .OrderByDescending(h => h.EnrollmentDate)
            .ToListAsync();

        return ObjectMapper.Map<List<EnrollmentHistoryDto>>(histories);
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
