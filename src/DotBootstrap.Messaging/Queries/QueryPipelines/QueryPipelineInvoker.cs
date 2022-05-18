using DotBootstrap.Messaging.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace DotBootstrap.Messaging.Queries.QueryPipelines;

internal interface IQueryPipelineInvoker
{
    IReadOnlyCollection<IQueryPreprocessor<TQuery, TResponse>> InvokePreprocessors<TQuery, TResponse>
        (TQuery query) where TQuery : IQuery<TResponse>;

    IReadOnlyCollection<IQueryPostprocessor<TQuery, TResponse>> InvokePostprocessors<TQuery, TResponse>
        (TQuery query) where TQuery : IQuery<TResponse>;

    IReadOnlyCollection<IQueryMiddleware<TQuery, TResponse>> InvokeMiddlewares<TQuery, TResponse>
        (TQuery query) where TQuery : IQuery<TResponse>;
}

internal class QueryPipelineInvoker : IQueryPipelineInvoker
{
    private readonly QueryPipelineStore _queryPipelineStore;
    private readonly IServiceProvider _serviceProvider;

    public QueryPipelineInvoker(QueryPipelineStore queryPipelineStore, IServiceProvider serviceProvider)
    {
        _queryPipelineStore = queryPipelineStore;
        _serviceProvider = serviceProvider;
    }

    public IReadOnlyCollection<IQueryPreprocessor<TQuery, TResponse>> InvokePreprocessors<TQuery, TResponse>
        (TQuery query) where TQuery : IQuery<TResponse>
    {
        return _queryPipelineStore.GetPreprocessors<TQuery, TResponse>(query)
            .Select(type => type.MakeGenericType(query.GetType(), typeof(TResponse)))
            .Select(type => _serviceProvider.GetRequiredService(type))
            .Cast<IQueryPreprocessor<TQuery, TResponse>>()
            .ToArray();
    }

    public IReadOnlyCollection<IQueryPostprocessor<TQuery, TResponse>> InvokePostprocessors<TQuery, TResponse>
        (TQuery query) where TQuery : IQuery<TResponse>
    {
        var res = _queryPipelineStore.GetPostprocessors<TQuery, TResponse>(query)
            .Select(type => type.MakeGenericType(query.GetType(), typeof(TResponse)))
            .Select(type => _serviceProvider.GetRequiredService(type))
            .Cast<IQueryPostprocessor<TQuery, TResponse>>()
            .ToArray();

        return res;
    }

    public IReadOnlyCollection<IQueryMiddleware<TQuery, TResponse>> InvokeMiddlewares<TQuery, TResponse>
        (TQuery query) where TQuery : IQuery<TResponse>
    {
        return _queryPipelineStore.GetMiddlewares<TQuery, TResponse>(query)
            .Select(type => type.MakeGenericType(query.GetType(), typeof(TResponse)))
            .Select(type => _serviceProvider.GetRequiredService(type))
            .Cast<IQueryMiddleware<TQuery, TResponse>>()
            .ToArray();
    }
}