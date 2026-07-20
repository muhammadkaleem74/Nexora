using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Nexora.Admissions.Dto;

namespace Nexora.Admissions;

public interface IStudentAppService
    : IAsyncCrudAppService<StudentListDto, long,
                           PagedStudentRequestDto,
                           CreateStudentDto,
                           UpdateStudentDto>
{
    Task<StudentDetailDto> GetDetailAsync(long id);

    Task<List<EnrollmentHistoryDto>> GetEnrollmentHistoryAsync(long studentId);
}
