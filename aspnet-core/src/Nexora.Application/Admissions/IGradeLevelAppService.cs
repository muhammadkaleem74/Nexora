using Abp.Application.Services;
using Nexora.Admissions.Dto;

namespace Nexora.Admissions;

public interface IGradeLevelAppService
    : IAsyncCrudAppService<GradeLevelDto, long, PagedGradeLevelRequestDto, GradeLevelDto, GradeLevelDto>
{
}
