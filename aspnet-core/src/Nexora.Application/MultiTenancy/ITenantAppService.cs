using Abp.Application.Services;
using Nexora.MultiTenancy.Dto;

namespace Nexora.MultiTenancy;

public interface ITenantAppService : IAsyncCrudAppService<TenantDto, int, PagedTenantResultRequestDto, CreateTenantDto, TenantDto>
{
}

