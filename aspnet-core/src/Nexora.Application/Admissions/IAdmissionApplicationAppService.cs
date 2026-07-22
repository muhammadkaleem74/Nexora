using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Nexora.Admissions.Dto;

namespace Nexora.Admissions;

public interface IAdmissionApplicationAppService
    : IAsyncCrudAppService<AdmissionApplicationListDto, long,
                           PagedAdmissionApplicationRequestDto,
                           CreateAdmissionApplicationDto,
                           UpdateAdmissionApplicationDto>
{
    Task<AdmissionApplicationDetailDto> GetDetailAsync(long id);

    Task SubmitApplicationAsync(long applicationId);

    Task ChangeStatusAsync(ChangeApplicationStatusDto input);

    Task<StudentListDto> ConvertToStudentAsync(ConvertToStudentDto input);

    // Guardians
    Task<ApplicationGuardianDto> AddGuardianAsync(long applicationId, CreateGuardianDto input);
    Task RemoveGuardianAsync(long applicationGuardianId);
    Task<List<ApplicationGuardianDto>> GetGuardiansAsync(long applicationId);

    // Documents
    Task<ApplicationDocumentDto> AddDocumentAsync(long applicationId, CreateApplicationDocumentDto input);
    Task RemoveDocumentAsync(long documentId);
    Task<ApplicationDocumentDto> VerifyDocumentAsync(long documentId, VerifyDocumentDto input);
    Task<List<ApplicationDocumentDto>> GetDocumentsAsync(long applicationId);

    // Assessments
    Task<AdmissionAssessmentDto> ScheduleAssessmentAsync(long applicationId, CreateAssessmentDto input);
    Task<AdmissionAssessmentDto> RecordAssessmentResultAsync(long assessmentId, RecordAssessmentResultDto input);
    Task<List<AdmissionAssessmentDto>> GetAssessmentsAsync(long applicationId);
}
