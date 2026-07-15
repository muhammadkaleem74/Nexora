using Abp.Zero.EntityFrameworkCore;
using Nexora.Authorization.Roles;
using Nexora.Authorization.Users;
using Nexora.MultiTenancy;
using Microsoft.EntityFrameworkCore;

namespace Nexora.EntityFrameworkCore;

public class NexoraDbContext : AbpZeroDbContext<Tenant, Role, User, NexoraDbContext>
{
    /* Define a DbSet for each entity of the application */

    public NexoraDbContext(DbContextOptions<NexoraDbContext> options)
        : base(options)
    {
    }
}
