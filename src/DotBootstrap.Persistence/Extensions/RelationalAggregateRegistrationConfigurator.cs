using DotBootstrap.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace DotBootstrap.Persistence.Extensions;

public class RelationalAggregateRegistrationConfigurator<TEntity> where TEntity : class
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
}