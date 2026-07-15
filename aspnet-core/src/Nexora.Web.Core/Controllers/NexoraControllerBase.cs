using Abp.AspNetCore.Mvc.Controllers;
using Abp.IdentityFramework;
using Microsoft.AspNetCore.Identity;

namespace Nexora.Controllers
{
    public abstract class NexoraControllerBase : AbpController
    {
        protected NexoraControllerBase()
        {
            LocalizationSourceName = NexoraConsts.LocalizationSourceName;
        }

        protected void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}
