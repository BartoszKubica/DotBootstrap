using DotBootstrap.Messaging.Commands;
using DotBootstrap.Messaging.Contracts;
using DotBootstrap.Messaging.Queries.QueryPipelines;
using Microsoft.Extensions.DependencyInjection;

namespace DotBootstrap.Messaging.Queries;
public interface IQueryPipelineConfigurator
{
    IQueryPipelineConfigurator AddGlobalPreprocessor(Type preprocessor);
    IQueryPipelineConfigurator AddGlobalPostprocessor(Type postprocessor);
    IQueryPipelineConfigurator AddGlobalMiddleware(Type middleware);

    IQueryPipelineConfigurator AddPreprocessorForQuery<TQuery, TResponse>(Type preprocessor)
        where TQuery : IQuery<TResponse>;

    IQueryPipelineConfigurator AddPostprocessorForQuery<TQuery, TResponse>(Type postprocessor)
        where TQuery : IQuery<TResponse>;

    IQueryPipelineConfigurator AddMiddlewareForQuery<TQuery, TResponse>(Type middleware)
        where TQuery : IQuery<TResponse>;
}

internal class QueryPipelineConfigurator : IQueryPipelineConfigurator
{
    private readonly QueryPipelineStore _queryPipelineStore = new ();
    private readonly HashSet<Type> _preprocessors = new();
    private readonly HashSet<Type> _postprocessors = new();
    private readonly HashSet<Type> _middlewares = new();
    private readonly IServiceCollection _serviceCollection;

    public QueryPipelineConfigurator(IServiceCollection serviceCollection)
    {
        _serviceCollection = serviceCollection;
    }

    public IQueryPipelineConfigurator AddGlobalPreprocessor(Type preprocessor)
    {
        preprocessor.IsQueryPreprocessor();
        _queryPipelineStore.AddGlobalPreprocessor(preprocessor);
        if (!_preprocessors.Contains(preprocessor))
        {
            _preprocessors.Add(preprocessor);
            _serviceCollection.AddScoped(preprocessor, preprocessor);
        }
            
        return this;
    }

    public IQueryPipelineConfigurator AddGlobalPostprocessor(Type postprocessor)
    {
        postprocessor.IsQueryPostprocessor();
        _queryPipelineStore.AddGlobalPostprocessor(postprocessor);
        if (!_postprocessors.Contains(postprocessor))
        {
            _postprocessors.Add(postprocessor);
            _serviceCollection.AddScoped(postprocessor, postprocessor);
        }
        return this;
    }

    public IQueryPipelineConfigurator AddGlobalMiddleware(Type middleware)
    {
        middleware.IsQueryMiddleware();
        _queryPipelineStore.AddGlobalMiddleware(middleware);
        if (!_middlewares.Contains(middleware))
        {
            _middlewares.Add(middleware);
            _serviceCollection.AddScoped(middleware, middleware);
        }
        return this;
    }

    public IQueryPipelineConfigurator AddPreprocessorForQuery<TQuery, TResponse>(Type preprocessor)
        where TQuery : IQuery<TResponse>
    {
        preprocessor.IsQueryPreprocessor();
        _queryPipelineStore.AddPreprocessorForQuery<TQuery, TResponse>(preprocessor);
        if (!_preprocessors.Contains(preprocessor))
        {
            _preprocessors.Add(preprocessor);
            _serviceCollection.AddScoped(preprocessor, preprocessor);
        }

        return this;
    }
    
    public IQueryPipelineConfigurator AddPostprocessorForQuery<TQuery, TResponse>(Type postprocessor)
        where TQuery : IQuery<TResponse>

    {
        postprocessor.IsQueryPostprocessor();
        _queryPipelineStore.AddPostprocessorForQuery<TQuery, TResponse>(postprocessor);
        if (!_postprocessors.Contains(postprocessor))
        {
            _postprocessors.Add(postprocessor);
            _serviceCollection.AddScoped(postprocessor, postprocessor);
        }
        
        return this;
    }
    
    public IQueryPipelineConfigurator AddMiddlewareForQuery<TQuery, TResponse>(Type middleware)
        where TQuery : IQuery<TResponse>

    {
        middleware.IsQueryMiddleware();
        _queryPipelineStore.AddMiddlewareForQuery<TQuery, TResponse>(middleware);
        if (!_middlewares.Contains(middleware))
        {
            _middlewares.Add(middleware);
            _serviceCollection.AddScoped(middleware, middleware);
        }
        return this;
    }

    public QueryPipelineStore Configure()
    {
        return _queryPipelineStore;
    }
}