using DotBootstrap.Messaging.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace DotBootstrap.Messaging.Queries;

public interface IQueryBus
{
    Task<TResponse> Send<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken);
}

internal class QueryBus : IQueryBus
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IQueryPipelineRunner _queryPipelineRunner;
    public QueryBus(IServiceProvider serviceProvider, IQueryPipelineRunner queryPipelineRunner)
    {
        _serviceProvider = serviceProvider;
        _queryPipelineRunner = queryPipelineRunner;
    }

    public async Task<TResponse> Send<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken)
    {
        var queryType = query.GetType();
        var genericHandlerType = typeof(IQueryHandler<,>).MakeGenericType(queryType, typeof(TResponse));
        var queryWrapperType = typeof(QueryWrapper<,>).MakeGenericType(queryType, typeof(TResponse));

        var handler = _serviceProvider.GetRequiredService(genericHandlerType);
        
        var wrapper = (QueryWrapperBase<TResponse>?)Activator.CreateInstance(queryWrapperType, handler, _queryPipelineRunner)
                      ?? throw new NullReferenceException("Could not create command handler");

        return await wrapper.Process(query, cancellationToken);
    }
}