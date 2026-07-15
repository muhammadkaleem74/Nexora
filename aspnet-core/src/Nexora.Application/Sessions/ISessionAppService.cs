using Abp.Application.Services;
using Nexora.Sessions.Dto;
using System.Threading.Tasks;

namespace Nexora.Sessions;

public interface ISessionAppService : IApplicationService
{
    Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
}
