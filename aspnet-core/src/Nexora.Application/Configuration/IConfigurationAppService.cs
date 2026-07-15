using Nexora.Configuration.Dto;
using System.Threading.Tasks;

namespace Nexora.Configuration;

public interface IConfigurationAppService
{
    Task ChangeUiTheme(ChangeUiThemeInput input);
}
