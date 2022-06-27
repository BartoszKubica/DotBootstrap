using DotBootstrap.Domain.DataPartitioning;
using Microsoft.Extensions.DependencyInjection;

namespace DotBootstrap.Domain;

public static class ServiceCollectionExtensions
{
    public static void AddDotBootstrap(this IServiceCollection services)
    {
        var tenantProvider = new TenantProvider();
        services.AddScoped<ITenantContext>(_ => tenantProvider);
        services.AddScoped<ITenantSetter>(_ => tenantProvider);
    }
}