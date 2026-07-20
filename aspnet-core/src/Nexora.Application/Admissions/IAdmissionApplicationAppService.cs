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
}
