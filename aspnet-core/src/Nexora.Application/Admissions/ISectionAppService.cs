using Abp.Application.Services;
using Nexora.Admissions.Dto;

namespace Nexora.Admissions;

public interface ISectionAppService
    : IAsyncCrudAppService<SectionDto, long, PagedSectionRequestDto, SectionDto, SectionDto>
{
}
