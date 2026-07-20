using Abp.Application.Services;
using Nexora.Admissions.Dto;

namespace Nexora.Admissions;

public interface ICampusAppService
    : IAsyncCrudAppService<CampusDto, long, PagedCampusRequestDto, CampusDto, CampusDto>
{
}
