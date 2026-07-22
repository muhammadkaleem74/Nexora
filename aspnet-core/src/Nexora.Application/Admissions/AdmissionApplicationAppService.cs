using System;
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
    private readonly IRepository<ApplicationDocument, long> _documentRepository;
    private readonly IRepository<AdmissionAssessment, long> _assessmentRepository;
    private readonly GuardianManager _guardianManager;

    public AdmissionApplicationAppService(
        IRepository<AdmissionApplication, long> repository,
        IRepository<ApplicationGuardian, long> applicationGuardianRepository,
        IRepository<Student, long> studentRepository,
        IRepository<StudentGuardian, long> studentGuardianRepository,
        IRepository<EnrollmentHistory, long> enrollmentHistoryRepository,
        IRepository<ApplicationDocument, long> documentRepository,
        IRepository<AdmissionAssessment, long> assessmentRepository,
        GuardianManager guardianManager)
        : base(repository)
    {
        _applicationGuardianRepository = applicationGuardianRepository;
        _studentRepository = studentRepository;
        _studentGuardianRepository = studentGuardianRepository;
        _enrollmentHistoryRepository = enrollmentHistoryRepository;
        _documentRepository = documentRepository;
        _assessmentRepository = assessmentRepository;
        _guardianManager = guardianManager;
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

    // ── Guardians ────────────────────────────────────────────────────────────

    [AbpAuthorize(PermissionNames.Pages_Admissions_Applications_ManageGuardians)]
    public async Task<ApplicationGuardianDto> AddGuardianAsync(long applicationId, CreateGuardianDto input)
    {
        var application = await Repository.GetAsync(applicationId);
        var guardian = await _guardianManager.FindOrCreateAsync(input);

        if (input.IsPrimaryContact)
            await _guardianManager.ClearPrimaryContactsAsync(
                _applicationGuardianRepository,
                ag => ag.ApplicationId == applicationId);

        var link = new ApplicationGuardian
        {
            ApplicationId = application.Id,
            GuardianId = guardian.Id,
            IsPrimaryContact = input.IsPrimaryContact
        };

        await _applicationGuardianRepository.InsertAsync(link);
        await CurrentUnitOfWork.SaveChangesAsync();

        return new ApplicationGuardianDto
        {
            Id = link.Id,
            GuardianId = guardian.Id,
            FullName = guardian.FullName,
            Relationship = guardian.Relationship,
            NationalIdNumber = guardian.NationalIdNumber,
            Email = guardian.Email,
            Phone = guardian.Phone,
            Occupation = guardian.Occupation,
            IsPrimaryContact = link.IsPrimaryContact
        };
    }

    [AbpAuthorize(PermissionNames.Pages_Admissions_Applications_ManageGuardians)]
    public async Task RemoveGuardianAsync(long applicationGuardianId)
    {
        await _applicationGuardianRepository.DeleteAsync(applicationGuardianId);
    }

    public async Task<List<ApplicationGuardianDto>> GetGuardiansAsync(long applicationId)
    {
        var links = await _applicationGuardianRepository.GetAll()
            .Include(ag => ag.Guardian)
            .Where(ag => ag.ApplicationId == applicationId)
            .ToListAsync();

        return links.Select(ag => new ApplicationGuardianDto
        {
            Id = ag.Id,
            GuardianId = ag.GuardianId,
            FullName = ag.Guardian.FullName,
            Relationship = ag.Guardian.Relationship,
            NationalIdNumber = ag.Guardian.NationalIdNumber,
            Email = ag.Guardian.Email,
            Phone = ag.Guardian.Phone,
            Occupation = ag.Guardian.Occupation,
            IsPrimaryContact = ag.IsPrimaryContact
        }).ToList();
    }

    // ── Documents ────────────────────────────────────────────────────────────

    [AbpAuthorize(PermissionNames.Pages_Admissions_Applications_ManageDocuments)]
    public async Task<ApplicationDocumentDto> AddDocumentAsync(long applicationId, CreateApplicationDocumentDto input)
    {
        var doc = new ApplicationDocument
        {
            ApplicationId = applicationId,
            DocumentType = input.DocumentType,
            FileName = input.FileName,
            FileUrl = input.FileUrl,
            FileSizeKb = input.FileSizeKb,
            UploadedDate = DateTime.Now,
            VerificationStatus = VerificationStatus.Pending
        };

        await _documentRepository.InsertAsync(doc);
        await CurrentUnitOfWork.SaveChangesAsync();

        return ObjectMapper.Map<ApplicationDocumentDto>(doc);
    }

    [AbpAuthorize(PermissionNames.Pages_Admissions_Applications_ManageDocuments)]
    public async Task RemoveDocumentAsync(long documentId)
    {
        await _documentRepository.DeleteAsync(documentId);
    }

    [AbpAuthorize(PermissionNames.Pages_Admissions_Applications_VerifyDocument)]
    public async Task<ApplicationDocumentDto> VerifyDocumentAsync(long documentId, VerifyDocumentDto input)
    {
        var doc = await _documentRepository.GetAsync(documentId);
        doc.VerificationStatus = input.VerificationStatus;
        doc.VerifiedByUserId = AbpSession.UserId;
        doc.VerifiedDate = DateTime.Now;
        doc.RejectionNote = input.RejectionNote;
        return ObjectMapper.Map<ApplicationDocumentDto>(doc);
    }

    public async Task<List<ApplicationDocumentDto>> GetDocumentsAsync(long applicationId)
    {
        var docs = await _documentRepository.GetAll()
            .Where(d => d.ApplicationId == applicationId)
            .OrderBy(d => d.DocumentType)
            .ToListAsync();

        return ObjectMapper.Map<List<ApplicationDocumentDto>>(docs);
    }

    // ── Assessments ──────────────────────────────────────────────────────────

    [AbpAuthorize(PermissionNames.Pages_Admissions_Applications_ManageAssessments)]
    public async Task<AdmissionAssessmentDto> ScheduleAssessmentAsync(long applicationId, CreateAssessmentDto input)
    {
        var assessment = new AdmissionAssessment
        {
            ApplicationId = applicationId,
            AssessmentType = input.AssessmentType,
            ScheduledDate = input.ScheduledDate,
            Result = AssessmentResult.Pending
        };

        await _assessmentRepository.InsertAsync(assessment);
        await CurrentUnitOfWork.SaveChangesAsync();

        return ObjectMapper.Map<AdmissionAssessmentDto>(assessment);
    }

    [AbpAuthorize(PermissionNames.Pages_Admissions_Applications_RecordAssessment)]
    public async Task<AdmissionAssessmentDto> RecordAssessmentResultAsync(long assessmentId, RecordAssessmentResultDto input)
    {
        var assessment = await _assessmentRepository.GetAsync(assessmentId);
        assessment.Score = input.Score;
        assessment.MaxScore = input.MaxScore;
        assessment.Remarks = input.Remarks;
        assessment.Result = input.Result;
        assessment.ConductedByUserId = AbpSession.UserId;
        return ObjectMapper.Map<AdmissionAssessmentDto>(assessment);
    }

    public async Task<List<AdmissionAssessmentDto>> GetAssessmentsAsync(long applicationId)
    {
        var assessments = await _assessmentRepository.GetAll()
            .Where(a => a.ApplicationId == applicationId)
            .OrderBy(a => a.ScheduledDate)
            .ToListAsync();

        return ObjectMapper.Map<List<AdmissionAssessmentDto>>(assessments);
    }

    // ── Query helpers ────────────────────────────────────────────────────────

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
