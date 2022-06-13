using DotBootstrap.Domain;
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

    public RelationalAggregateRegistrationConfigurator<TEntity> WithDecorator<TDecorator>()
        where TDecorator : IRepository<TEntity>
    {
        _serviceCollection.Decorate(
            serviceType: typeof(IRepository<>).MakeGenericType(typeof(TEntity)),
            decoratorType: typeof(TDecorator).MakeGenericType(typeof(TEntity)));

        return this;
    }
}