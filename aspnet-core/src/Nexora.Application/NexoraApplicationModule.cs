using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Nexora.Authorization;

namespace Nexora;

[DependsOn(
    typeof(NexoraCoreModule),
    typeof(AbpAutoMapperModule))]
public class NexoraApplicationModule : AbpModule
{
    public override void PreInitialize()
    {
        Configuration.Authorization.Providers.Add<NexoraAuthorizationProvider>();
    }

    public override void Initialize()
    {
        var thisAssembly = typeof(NexoraApplicationModule).GetAssembly();

        IocManager.RegisterAssemblyByConvention(thisAssembly);

        Configuration.Modules.AbpAutoMapper().Configurators.Add(
            // Scan the assembly for classes which inherit from AutoMapper.Profile
            cfg => cfg.AddMaps(thisAssembly)
        );
    }
}
