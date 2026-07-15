using Abp.Application.Services;
using Nexora.Authorization.Accounts.Dto;
using System.Threading.Tasks;

namespace Nexora.Authorization.Accounts;

public interface IAccountAppService : IApplicationService
{
    Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input);

    Task<RegisterOutput> Register(RegisterInput input);
}
