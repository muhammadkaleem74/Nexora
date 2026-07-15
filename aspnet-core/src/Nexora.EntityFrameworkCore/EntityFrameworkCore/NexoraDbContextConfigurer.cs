using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace Nexora.EntityFrameworkCore;

public static class NexoraDbContextConfigurer
{
    public static void Configure(DbContextOptionsBuilder<NexoraDbContext> builder, string connectionString)
    {
        builder.UseSqlServer(connectionString);
    }

    public static void Configure(DbContextOptionsBuilder<NexoraDbContext> builder, DbConnection connection)
    {
        builder.UseSqlServer(connection);
    }
}
