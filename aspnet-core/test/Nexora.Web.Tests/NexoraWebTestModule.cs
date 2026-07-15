using Abp.AspNetCore;
using Abp.AspNetCore.TestBase;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Nexora.EntityFrameworkCore;
using Nexora.Web.Startup;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace Nexora.Web.Tests;

[DependsOn(
    typeof(NexoraWebMvcModule),
    typeof(AbpAspNetCoreTestBaseModule)
)]
public class NexoraWebTestModule : AbpModule
{
    public NexoraWebTestModule(NexoraEntityFrameworkModule abpProjectNameEntityFrameworkModule)
    {
        abpProjectNameEntityFrameworkModule.SkipDbContextRegistration = true;
    }

    public override void PreInitialize()
    {
        Configuration.UnitOfWork.IsTransactional = false; //EF Core InMemory DB does not support transactions.
    }

    public override void Initialize()
    {
        IocManager.RegisterAssemblyByConvention(typeof(NexoraWebTestModule).GetAssembly());
    }

    public override void PostInitialize()
    {
        IocManager.Resolve<ApplicationPartManager>()
            .AddApplicationPartsIfNotAddedBefore(typeof(NexoraWebMvcModule).Assembly);
    }
}