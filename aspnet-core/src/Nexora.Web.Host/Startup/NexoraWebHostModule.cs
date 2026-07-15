using Abp.Modules;
using Abp.Reflection.Extensions;
using Nexora.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Nexora.Web.Host.Startup
{
    [DependsOn(
       typeof(NexoraWebCoreModule))]
    public class NexoraWebHostModule : AbpModule
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public NexoraWebHostModule(IWebHostEnvironment env)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(NexoraWebHostModule).GetAssembly());
        }
    }
}
