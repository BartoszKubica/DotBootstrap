using DotBootstrap.Domain;
using DotBootstrap.Domain.DataPartitioning;
using DotBootstrap.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace DotBootstrap.Persistence.Extensions;

public class RelationalAggregateRegistrationConfigurator<TEntity>
    where TEntity : Aggregate
{
    private readonly IServiceCollection _serviceCollection;

    public RelationalAggregateRegistrationConfigurator(IServiceCollection serviceCollection)
    {
        _serviceCollection = serviceCollection;
    }

    public RelationalAggregateRegistrationConfigurator<TEntity> WithQuery(
        Func<IQueryable<TEntity>, IQueryable<TEntity>> queryTransformation)
    {
        _serviceCollection.AddSingleton(queryTransformation);
        return this;
    }

    public RelationalAggregateRegistrationConfigurator<TEntity> WithTenantGuardDecorator()
    {
        if (typeof(TEntity).IsAssignableTo(typeof(ITenantEntity)))
            throw new ArgumentException($"{nameof(TEntity)} is not implementing ITenantEntity interface.");
        
        _serviceCollection.Decorate<IRepository<TEntity>, TenantRepositoryDecorator<TEntity>>();

        return this;
    }
}