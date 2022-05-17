using System.Collections.Concurrent;
using DotBootstrap.Messaging.Contracts;

namespace DotBootstrap.Messaging.Queries.QueryPipelines;

internal interface IQueryPipelineStore
{
    void AddGlobalPreprocessor(Type commandPreprocessor);
    void AddGlobalPostprocessor(Type commandPostprocessor);
    void AddGlobalMiddleware(Type commandMiddleware);
    void AddMiddlewareForQuery<TQuery, TResponse>(Type queryMiddleware) where TQuery : IQuery<TResponse>;
    void AddPreprocessorForQuery<TQuery, TResponse>(Type queryPreprocessor) where TQuery : IQuery<TResponse>;
    void AddPostprocessorForQuery<TQuery, TResponse>(Type queryPreprocessor) where TQuery : IQuery<TResponse>;

    IReadOnlyCollection<Type> GetPreprocessors<TQuery, TResponse>(TQuery query)
        where TQuery : IQuery<TResponse>;

    IReadOnlyCollection<Type> GetMiddlewares<TQuery, TResponse>(TQuery query)
        where TQuery : IQuery<TResponse>;

    IReadOnlyCollection<Type> GetPostprocessors<TQuery, TResponse>(TQuery query)
        where TQuery : IQuery<TResponse>;
}

internal class QueryPipelineStore : IQueryPipelineStore
{
    private readonly IList<Type> _globalPreprocessors = new List<Type>();
    private readonly IList<Type> _globalPostprocessor = new List<Type>();
    private readonly IList<Type> _globalMiddlewares = new List<Type>();
    private readonly ConcurrentDictionary<Type, IList<Type>> _queryPreprocessors = new();
    private readonly ConcurrentDictionary<Type, IList<Type>> _queryPostprocessors = new();
    private readonly ConcurrentDictionary<Type, IList<Type>> _queryMiddlewares = new();
    public void AddGlobalPreprocessor(Type commandPreprocessor)
    {
        _globalPreprocessors.Add(commandPreprocessor);
    }
    
    public void AddGlobalPostprocessor(Type commandPostprocessor)
    {
        _globalPostprocessor.Add(commandPostprocessor);
    }

    public void AddGlobalMiddleware(Type commandMiddleware)
    {
        _globalMiddlewares.Add(commandMiddleware);
    }

    public void AddMiddlewareForQuery<TQuery, TResponse>(Type queryMiddleware) where TQuery : IQuery<TResponse>
    {
        var queryType = typeof(TQuery);
        _queryMiddlewares.TryGetValue(queryType, out var pipeline);

        if (pipeline is null)
        {
            List<Type> list = new() { queryMiddleware };
            _queryMiddlewares.TryAdd(queryType, list);
        }
        else
        {
            pipeline.Add(queryMiddleware);
        }
    }

    public void AddPreprocessorForQuery<TQuery, TResponse>(Type queryPreprocessor) where TQuery : IQuery<TResponse>
    {
        var queryType = typeof(TQuery);
        _queryPreprocessors.TryGetValue(queryType, out var pipeline);

        if (pipeline is null)
        {
            List<Type> list = new() { queryPreprocessor };
            _queryPreprocessors.TryAdd(queryType, list);
        }
        else
        {
            pipeline.Add(queryPreprocessor);
        }
    }
    
    public void AddPostprocessorForQuery<TQuery, TResponse>(Type queryPostprocessor) where TQuery : IQuery<TResponse>
    {
        var queryType = typeof(TQuery);
        _queryPostprocessors.TryGetValue(queryType, out var pipeline);

        if (pipeline is null)
        {
            List<Type> list = new() { queryPostprocessor };
            _queryPostprocessors.TryAdd(queryType, list);
        }
        else
        {
            pipeline.Add(queryType);
        }
    }
    
    public IReadOnlyCollection<Type> GetPreprocessors<TQuery, TResponse>(TQuery query)
        where TQuery : IQuery<TResponse>
    {
        var queryType = query.GetType();

        var source = _queryPreprocessors.ContainsKey(queryType)
            ? _queryPreprocessors[queryType]
            : _globalPreprocessors;
        return source.ToArray();
    }

    public IReadOnlyCollection<Type> GetMiddlewares<TQuery, TResponse>(TQuery query)
        where TQuery : IQuery<TResponse>
    {
        var queryType = query.GetType();
        var source = _queryMiddlewares.ContainsKey(queryType)
            ? _queryMiddlewares[queryType]
            : _globalMiddlewares;

        return source.ToArray();
    }

    public IReadOnlyCollection<Type> GetPostprocessors<TQuery, TResponse>(TQuery query)
        where TQuery : IQuery<TResponse>
    {
        var queryType = query.GetType();
        
        var source = _queryPostprocessors.ContainsKey(queryType)
            ? _queryPostprocessors[queryType]
            : _globalPostprocessor;

        return source.ToArray();
    }
}