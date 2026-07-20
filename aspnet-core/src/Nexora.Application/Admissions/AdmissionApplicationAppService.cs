using System;
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
using Nexora.Domain.Admissions.Enums;
using System.Linq.Dynamic.Core;

namespace Nexora.Admissions;

[AbpAuthorize(PermissionNames.Pages_Admissions_Applications)]
public class AdmissionApplicationAppService
    : AsyncCrudAppService<AdmissionApplication, AdmissionApplicationListDto, long,
                          PagedAdmissionApplicationRequestDto,
                          CreateAdmissionApplicationDto,
                          UpdateAdmissionApplicationDto>,
      IAdmissionApplicationAppService
{
    private readonly IRepository<ApplicationGuardian, long> _applicationGuardianRepository;
    private readonly IRepository<Student, long> _studentRepository;
    private readonly IRepository<StudentGuardian, long> _studentGuardianRepository;
    private readonly IRepository<EnrollmentHistory, long> _enrollmentHistoryRepository;

    public AdmissionApplicationAppService(
        IRepository<AdmissionApplication, long> repository,
        IRepository<ApplicationGuardian, long> applicationGuardianRepository,
        IRepository<Student, long> studentRepository,
        IRepository<StudentGuardian, long> studentGuardianRepository,
        IRepository<EnrollmentHistory, long> enrollmentHistoryRepository)
        : base(repository)
    {
        _applicationGuardianRepository = applicationGuardianRepository;
        _studentRepository = studentRepository;
        _studentGuardianRepository = studentGuardianRepository;
        _enrollmentHistoryRepository = enrollmentHistoryRepository;
    }

    [AbpAuthorize(PermissionNames.Pages_Admissions_Applications_Create)]
    public override async Task<AdmissionApplicationListDto> CreateAsync(CreateAdmissionApplicationDto input)
    {
        CheckCreatePermission();

        var application = ObjectMapper.Map<AdmissionApplication>(input);
        application.ApplicationNumber = await GenerateApplicationNumberAsync();
        application.Status = ApplicationStatus.Draft;
        application.SubmittedDate = DateTime.Now;

        await Repository.InsertAsync(application);
        await CurrentUnitOfWork.SaveChangesAsync();

        return MapToEntityDto(application);
    }

    [AbpAuthorize(PermissionNames.Pages_Admissions_Applications_Edit)]
    public override async Task<AdmissionApplicationListDto> UpdateAsync(UpdateAdmissionApplicationDto input)
    {
        var application = await Repository.GetAsync(input.Id);

        if (application.Status != ApplicationStatus.Draft)
            throw new UserFriendlyException("Only draft applications can be edited directly.");

        ObjectMapper.Map(input, application);
        return MapToEntityDto(application);
    }

    [AbpAuthorize(PermissionNames.Pages_Admissions_Applications_Delete)]
    public override async Task DeleteAsync(EntityDto<long> input)
    {
        var application = await Repository.GetAsync(input.Id);

        if (application.Status == ApplicationStatus.Enrolled)
            throw new UserFriendlyException("Cannot delete an application that has been converted to a student.");

        await Repository.DeleteAsync(input.Id);
    }

    public async Task<AdmissionApplicationDetailDto> GetDetailAsync(long id)
    {
        var application = await Repository.GetAll()
            .Include(a => a.AcademicYear)
            .Include(a => a.Campus)
            .Include(a => a.DesiredGradeLevel)
            .Include(a => a.ApplicationGuardians).ThenInclude(ag => ag.Guardian)
            .Include(a => a.Documents)
            .Include(a => a.Assessments)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (application == null)
            throw new UserFriendlyException("Application not found.");

        return ObjectMapper.Map<AdmissionApplicationDetailDto>(application);
    }

    [AbpAuthorize(PermissionNames.Pages_Admissions_Applications_ChangeStatus)]
    public async Task SubmitApplicationAsync(long applicationId)
    {
        var application = await Repository.GetAsync(applicationId);

        if (application.Status != ApplicationStatus.Draft)
            throw new UserFriendlyException("Only draft applications can be submitted.");

        application.Status = ApplicationStatus.Submitted;
        application.SubmittedDate = DateTime.Now;
    }

    [AbpAuthorize(PermissionNames.Pages_Admissions_Applications_ChangeStatus)]
    public async Task ChangeStatusAsync(ChangeApplicationStatusDto input)
    {
        var application = await Repository.GetAsync(input.ApplicationId);

        application.Status = input.NewStatus;
        application.ReviewedByUserId = AbpSession.UserId;

        if (!input.Notes.IsNullOrWhiteSpace())
            application.ReviewNotes = input.Notes;

        if (input.NewStatus == ApplicationStatus.Rejected && !input.RejectionReason.IsNullOrWhiteSpace())
            application.RejectionReason = input.RejectionReason;
    }

    [AbpAuthorize(PermissionNames.Pages_Admissions_Applications_ChangeStatus)]
    public async Task<StudentListDto> ConvertToStudentAsync(ConvertToStudentDto input)
    {
        var application = await Repository.GetAll()
            .Include(a => a.ApplicationGuardians)
            .FirstOrDefaultAsync(a => a.Id == input.ApplicationId);

        if (application == null)
            throw new UserFriendlyException("Application not found.");

        if (application.Status != ApplicationStatus.Approved)
            throw new UserFriendlyException("Only approved applications can be converted to students.");

        var student = new Student
        {
            ApplicationId = application.Id,
            RegistrationNumber = input.RegistrationNumber,
            GRNumber = input.GRNumber,
            FirstName = application.FirstName,
            LastName = application.LastName,
            DateOfBirth = application.DateOfBirth,
            Gender = application.Gender,
            Nationality = application.Nationality,
            Religion = application.Religion,
            CampusId = input.CampusId,
            CurrentGradeLevelId = input.CurrentGradeLevelId,
            CurrentSectionId = input.CurrentSectionId,
            EnrollmentDate = input.EnrollmentDate,
            Status = StudentStatus.Active
        };

        var studentId = await _studentRepository.InsertAndGetIdAsync(student);

        // Copy guardians from application to student
        foreach (var ag in application.ApplicationGuardians)
        {
            await _studentGuardianRepository.InsertAsync(new StudentGuardian
            {
                StudentId = studentId,
                GuardianId = ag.GuardianId,
                IsPrimaryContact = ag.IsPrimaryContact,
                CanPickupStudent = ag.IsPrimaryContact
            });
        }

        // Create initial enrollment history
        await _enrollmentHistoryRepository.InsertAsync(new EnrollmentHistory
        {
            StudentId = studentId,
            AcademicYearId = input.AcademicYearId,
            GradeLevelId = input.CurrentGradeLevelId,
            SectionId = input.CurrentSectionId,
            EnrollmentDate = input.EnrollmentDate,
            PromotionStatus = PromotionStatus.Promoted
        });

        application.Status = ApplicationStatus.Enrolled;

        await CurrentUnitOfWork.SaveChangesAsync();

        return ObjectMapper.Map<StudentListDto>(student);
    }

    protected override IQueryable<AdmissionApplication> CreateFilteredQuery(PagedAdmissionApplicationRequestDto input)
    {
        return Repository
            .GetAllIncluding(a => a.AcademicYear, a => a.Campus, a => a.DesiredGradeLevel)
            .WhereIf(!input.Keyword.IsNullOrWhiteSpace(),
                a => a.FirstName.Contains(input.Keyword) ||
                     a.LastName.Contains(input.Keyword) ||
                     a.ApplicationNumber.Contains(input.Keyword))
            .WhereIf(input.Status.HasValue, a => a.Status == input.Status.Value)
            .WhereIf(input.CampusId.HasValue, a => a.CampusId == input.CampusId.Value)
            .WhereIf(input.AcademicYearId.HasValue, a => a.AcademicYearId == input.AcademicYearId.Value);
    }

    protected override IQueryable<AdmissionApplication> ApplySorting(
        IQueryable<AdmissionApplication> query, PagedAdmissionApplicationRequestDto input)
    {
        if (input.Sorting.IsNullOrWhiteSpace())
            return query.OrderByDescending(a => a.SubmittedDate);

        return query.OrderBy(input.Sorting);
    }

    private async Task<string> GenerateApplicationNumberAsync()
    {
        var year = DateTime.Now.Year;
        var count = await Repository.CountAsync(a => a.CreationTime.Year == year);
        return $"APP-{year}-{(count + 1):D5}";
    }
}
