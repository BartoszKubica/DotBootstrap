using DotBootstrap.Domain;
using DotBootstrap.Domain.DataPartitioning;
using DotBootstrap.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace DotBootstrap.Persistence.Extensions;

public class RelationalAggregateRegistrationConfigurator
{
    private readonly IServiceCollection _serviceCollection;

    public RelationalAggregateRegistrationConfigurator(IServiceCollection serviceCollection)
    {
        _serviceCollection = serviceCollection;
    }

    public RelationalAggregateRegistrationConfigurator WithQuery<TEntity>(
        Func<IQueryable<TEntity>, IQueryable<TEntity>> queryTransformation) where TEntity : Aggregate
    {
        _serviceCollection.AddSingleton(queryTransformation);
        return this;
    }

    public RelationalAggregateRegistrationConfigurator WithTenantGuardDecorator<TEntity>() where TEntity : Aggregate, 
        ITenantEntity
    {
        _serviceCollection.Decorate<IRepository<TEntity>, TenantRepositoryDecorator<TEntity>>();

        return this;
    }
}