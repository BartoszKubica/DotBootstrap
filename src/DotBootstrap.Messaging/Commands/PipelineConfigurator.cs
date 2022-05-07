using System.Collections.Concurrent;
using DotBootstrap.Messaging.Commands.CommandPipelines;
using DotBootstrap.Messaging.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace DotBootstrap.Messaging.Commands;

public interface IPipelineConfigurator
{
    IPipelineConfigurator AddGlobalPreprocessor(Type preprocessor);
    IPipelineConfigurator AddGlobalPostprocessor(Type postprocessor);
    IPipelineConfigurator AddGlobalMiddleware(Type middleware);

    IPipelineConfigurator AddPreprocessorForCommand<TCommand>(Type preprocessor)
        where TCommand : ICommand;

    IPipelineConfigurator AddPostprocessorForCommand<TCommand>(Type postprocessor)
        where TCommand : ICommand;

    IPipelineConfigurator AddMiddlewareForCommand<TCommand>(Type middleware)
        where TCommand : ICommand;
}

internal class PipelineConfigurator : IPipelineConfigurator
{
    private readonly PipelineStore _pipelineStore = new ();
    private readonly HashSet<Type> _preprocessors = new();
    private readonly HashSet<Type> _postprocessors = new();
    private readonly HashSet<Type> _middlewares = new();
    private readonly IServiceCollection _serviceCollection;

    public PipelineConfigurator(IServiceCollection serviceCollection)
    {
        _serviceCollection = serviceCollection;
    }

    public IPipelineConfigurator AddGlobalPreprocessor(Type preprocessor)
    {
        preprocessor.IsCommandPreprocessor();
        _pipelineStore.AddGlobalPreprocessor(preprocessor);
        if (!_preprocessors.Contains(preprocessor))
        {
            _preprocessors.Add(preprocessor);
            _serviceCollection.AddScoped(preprocessor, preprocessor);
        }
            
        return this;
    }

    public IPipelineConfigurator AddGlobalPostprocessor(Type postprocessor)
    {
        postprocessor.IsCommandPostprocessor();
        _pipelineStore.AddGlobalPostprocessor(postprocessor);
        if (!_postprocessors.Contains(postprocessor))
        {
            _postprocessors.Add(postprocessor);
            _serviceCollection.AddScoped(postprocessor, postprocessor);
        }
        return this;
    }

    public IPipelineConfigurator AddGlobalMiddleware(Type middleware)
    {
        middleware.IsCommandMiddleware();
        _pipelineStore.AddGlobalMiddleware(middleware);
        if (!_middlewares.Contains(middleware))
        {
            _middlewares.Add(middleware);
            _serviceCollection.AddScoped(middleware, middleware);
        }
        return this;
    }

    public IPipelineConfigurator AddPreprocessorForCommand<TCommand>(Type preprocessor)
        where TCommand : ICommand
    {
        preprocessor.IsCommandPreprocessor();
        _pipelineStore.AddPreprocessorForCommand<TCommand>(preprocessor);
        if (!_preprocessors.Contains(preprocessor))
        {
            _preprocessors.Add(preprocessor);
            _serviceCollection.AddScoped(preprocessor, preprocessor);
        }

        return this;
    }
    
    public IPipelineConfigurator AddPostprocessorForCommand<TCommand>(Type postprocessor)
        where TCommand : ICommand
    {
        postprocessor.IsCommandPostprocessor();
        _pipelineStore.AddPostprocessorForCommand<TCommand>(postprocessor);
        if (!_postprocessors.Contains(postprocessor))
        {
            _postprocessors.Add(postprocessor);
            _serviceCollection.AddScoped(postprocessor, postprocessor);
        }
        
        return this;
    }
    
    public IPipelineConfigurator AddMiddlewareForCommand<TCommand>(Type middleware)
        where TCommand : ICommand
    {
        middleware.IsCommandMiddleware();
        _pipelineStore.AddMiddlewareForCommand<TCommand>(middleware);
        if (!_middlewares.Contains(middleware))
        {
            _middlewares.Add(middleware);
            _serviceCollection.AddScoped(middleware, middleware);
        }
        return this;
    }

    public PipelineStore Configure()
    {
        return _pipelineStore;
    }
}