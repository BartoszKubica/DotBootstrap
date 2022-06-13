using DotBootstrap.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DotBootstrap.Persistence.IntegrationTests;

internal static class DbContextProvider
{
    internal static void ConfigureTestDbContext(this IServiceCollection services)
    {
        services.ConfigureDbContext<TestDbContext>(x =>
        {
            x.UseNpgsql("User ID=user;Password=user;Host=localhost;Port=5432;Database=test-db;");
        });
    }
}