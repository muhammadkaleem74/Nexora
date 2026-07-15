using Abp.Authorization;
using Abp.Runtime.Session;
using Nexora.Configuration.Dto;
using System.Threading.Tasks;

namespace Nexora.Configuration;

[AbpAuthorize]
public class ConfigurationAppService : NexoraAppServiceBase, IConfigurationAppService
{
    public async Task ChangeUiTheme(ChangeUiThemeInput input)
    {
        await SettingManager.ChangeSettingForUserAsync(AbpSession.ToUserIdentifier(), AppSettingNames.UiTheme, input.Theme);
    }
}
