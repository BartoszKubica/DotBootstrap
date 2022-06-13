using DotBootstrap.Domain;
using DotBootstrap.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DotBootstrap.Persistence.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterAggregateRepository<TModel, TEntity, TMapper>(this IServiceCollection services, 
        Action<RelationalAggregateRegistrationConfigurator<TEntity>>? action = null) 
        where TEntity : Aggregate where TModel : class, IVersionedEntity where TMapper : class, IDomainMapper<TModel, TEntity>
    {
        services.AddScoped<IRepository<TEntity>, AggregateRepository<TModel, TEntity>>();
        services.AddScoped<IDomainMapper<TModel, TEntity>, TMapper>();
        if (action is null) return services;
        var configurator = new RelationalAggregateRegistrationConfigurator<TEntity>(services);
        action(configurator);

        return services;
    }

    public static IServiceCollection ConfigureDbContext<TContext>(this IServiceCollection services, 
        Action<DbContextOptionsBuilder> optionsBuilder) where TContext : DbContext
    {
        services.AddDbContext<TContext>(optionsBuilder);
        services.AddScoped<DbContext, TContext>();
        return services;
    }
}

