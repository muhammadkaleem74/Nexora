using Abp.Events.Bus;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Nexora.Configuration;
using Nexora.EntityFrameworkCore;
using Nexora.Migrator.DependencyInjection;
using Castle.MicroKernel.Registration;
using Microsoft.Extensions.Configuration;

namespace Nexora.Migrator;

[DependsOn(typeof(NexoraEntityFrameworkModule))]
public class NexoraMigratorModule : AbpModule
{
    private readonly IConfigurationRoot _appConfiguration;

    public NexoraMigratorModule(NexoraEntityFrameworkModule abpProjectNameEntityFrameworkModule)
    {
        abpProjectNameEntityFrameworkModule.SkipDbSeed = true;

        _appConfiguration = AppConfigurations.Get(
            typeof(NexoraMigratorModule).GetAssembly().GetDirectoryPathOrNull()
        );
    }

    public override void PreInitialize()
    {
        Configuration.DefaultNameOrConnectionString = _appConfiguration.GetConnectionString(
            NexoraConsts.ConnectionStringName
        );

        Configuration.BackgroundJobs.IsJobExecutionEnabled = false;
        Configuration.ReplaceService(
            typeof(IEventBus),
            () => IocManager.IocContainer.Register(
                Component.For<IEventBus>().Instance(NullEventBus.Instance)
            )
        );
    }

    public override void Initialize()
    {
        IocManager.RegisterAssemblyByConvention(typeof(NexoraMigratorModule).GetAssembly());
        ServiceCollectionRegistrar.Register(IocManager);
    }
}
