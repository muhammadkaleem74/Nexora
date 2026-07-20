using Abp.Application.Services;
using Nexora.Admissions.Dto;

namespace Nexora.Admissions;

public interface IAcademicYearAppService
    : IAsyncCrudAppService<AcademicYearDto, long, PagedAcademicYearRequestDto, AcademicYearDto, AcademicYearDto>
{
}
