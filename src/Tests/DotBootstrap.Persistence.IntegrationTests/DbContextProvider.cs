using DotBootstrap.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DotBootstrap.Persistence.IntegrationTests;

internal static class DbContextProvider
{
    internal static string ConnectionString = "User ID=user;Password=user;Host=localhost;Port=5432;Database=test-db;";
    internal static void ConfigureTestDbContext(this IServiceCollection services)
    {
        services.ConfigureDbContext<TestDbContext>(x =>
        {
            x.UseNpgsql(ConnectionString);
        });
    }
}