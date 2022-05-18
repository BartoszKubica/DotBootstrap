using System.Collections.Concurrent;
using DotBootstrap.Messaging.Commands.CommandPipelines;
using DotBootstrap.Messaging.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace DotBootstrap.Messaging.Commands;

public interface ICommandPipelineConfigurator
{
    ICommandPipelineConfigurator AddGlobalPreprocessor(Type preprocessor);
    ICommandPipelineConfigurator AddGlobalPostprocessor(Type postprocessor);
    ICommandPipelineConfigurator AddGlobalMiddleware(Type middleware);

    ICommandPipelineConfigurator AddPreprocessorForCommand<TCommand>(Type preprocessor)
        where TCommand : ICommand;

    ICommandPipelineConfigurator AddPostprocessorForCommand<TCommand>(Type postprocessor)
        where TCommand : ICommand;

    ICommandPipelineConfigurator AddMiddlewareForCommand<TCommand>(Type middleware)
        where TCommand : ICommand;
}

internal class CommandPipelineConfigurator : ICommandPipelineConfigurator
{
    private readonly CommandPipelineStore _commandPipelineStore = new ();
    private readonly HashSet<Type> _preprocessors = new();
    private readonly HashSet<Type> _postprocessors = new();
    private readonly HashSet<Type> _middlewares = new();
    private readonly IServiceCollection _serviceCollection;

    public CommandPipelineConfigurator(IServiceCollection serviceCollection)
    {
        _serviceCollection = serviceCollection;
    }

    public ICommandPipelineConfigurator AddGlobalPreprocessor(Type preprocessor)
    {
        preprocessor.IsCommandPreprocessor();
        _commandPipelineStore.AddGlobalPreprocessor(preprocessor);
        if (!_preprocessors.Contains(preprocessor))
        {
            _preprocessors.Add(preprocessor);
            _serviceCollection.AddScoped(preprocessor, preprocessor);
        }
            
        return this;
    }

    public ICommandPipelineConfigurator AddGlobalPostprocessor(Type postprocessor)
    {
        postprocessor.IsCommandPostprocessor();
        _commandPipelineStore.AddGlobalPostprocessor(postprocessor);
        if (!_postprocessors.Contains(postprocessor))
        {
            _postprocessors.Add(postprocessor);
            _serviceCollection.AddScoped(postprocessor, postprocessor);
        }
        return this;
    }

    public ICommandPipelineConfigurator AddGlobalMiddleware(Type middleware)
    {
        middleware.IsCommandMiddleware();
        _commandPipelineStore.AddGlobalMiddleware(middleware);
        if (!_middlewares.Contains(middleware))
        {
            _middlewares.Add(middleware);
            _serviceCollection.AddScoped(middleware, middleware);
        }
        return this;
    }

    public ICommandPipelineConfigurator AddPreprocessorForCommand<TCommand>(Type preprocessor)
        where TCommand : ICommand
    {
        preprocessor.IsCommandPreprocessor();
        _commandPipelineStore.AddPreprocessorForCommand<TCommand>(preprocessor);
        if (!_preprocessors.Contains(preprocessor))
        {
            _preprocessors.Add(preprocessor);
            _serviceCollection.AddScoped(preprocessor, preprocessor);
        }

        return this;
    }
    
    public ICommandPipelineConfigurator AddPostprocessorForCommand<TCommand>(Type postprocessor)
        where TCommand : ICommand
    {
        postprocessor.IsCommandPostprocessor();
        _commandPipelineStore.AddPostprocessorForCommand<TCommand>(postprocessor);
        if (!_postprocessors.Contains(postprocessor))
        {
            _postprocessors.Add(postprocessor);
            _serviceCollection.AddScoped(postprocessor, postprocessor);
        }
        
        return this;
    }
    
    public ICommandPipelineConfigurator AddMiddlewareForCommand<TCommand>(Type middleware)
        where TCommand : ICommand
    {
        middleware.IsCommandMiddleware();
        _commandPipelineStore.AddMiddlewareForCommand<TCommand>(middleware);
        if (!_middlewares.Contains(middleware))
        {
            _middlewares.Add(middleware);
            _serviceCollection.AddScoped(middleware, middleware);
        }
        return this;
    }

    public CommandPipelineStore Configure()
    {
        return _commandPipelineStore;
    }
}