using System.Collections.Concurrent;
using DotBootstrap.Messaging.Contracts;

namespace DotBootstrap.Messaging.Commands.CommandPipelines;

public interface IPipelineStore
{
    void AddGlobalPreprocessor(Type commandPreprocessor);
    void AddGlobalPostprocessor(Type commandPostprocessor);
    void AddGlobalMiddleware(Type commandMiddleware);
    void AddMiddlewareForCommand<TCommand>(Type commandMiddleware) where TCommand : ICommand;
    void AddPreprocessorForCommand<TCommand>(Type commandPreprocessor) where TCommand : ICommand;
    void AddPostprocessorForCommand<TCommand>(Type commandPreprocessor) where TCommand : ICommand;

    IReadOnlyCollection<Type> GetPreprocessors<TCommand>(TCommand command)
        where TCommand : ICommand;

    IReadOnlyCollection<Type> GetMiddlewares<TCommand>(TCommand command)
        where TCommand : ICommand;

    IReadOnlyCollection<Type> GetPostprocessors<TCommand>(TCommand command)
        where TCommand : ICommand;
}

internal class PipelineStore : IPipelineStore
{
    private readonly IList<Type> _globalPreprocessors = new List<Type>();
    private readonly IList<Type> _globalPostprocessor = new List<Type>();
    private readonly IList<Type> _globalMiddlewares = new List<Type>();
    private readonly ConcurrentDictionary<Type, IList<Type>> _commandPreprocessors = new();
    private readonly ConcurrentDictionary<Type, IList<Type>> _commandPostprocessors = new();
    private readonly ConcurrentDictionary<Type, IList<Type>> _commandMiddlewares = new();
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

    public void AddMiddlewareForCommand<TCommand>(Type commandMiddleware) where TCommand : ICommand
    {
        var commandType = typeof(TCommand);
        _commandMiddlewares.TryGetValue(commandType, out var pipeline);

        if (pipeline is null)
        {
            List<Type> list = new() { commandMiddleware };
            _commandMiddlewares.TryAdd(commandType, list);
        }
        else
        {
            pipeline.Add(commandMiddleware);
        }
    }

    public void AddPreprocessorForCommand<TCommand>(Type commandPreprocessor) where TCommand : ICommand
    {
        var commandType = typeof(TCommand);
        _commandPreprocessors.TryGetValue(commandType, out var pipeline);

        if (pipeline is null)
        {
            List<Type> list = new() { commandPreprocessor };
            _commandPreprocessors.TryAdd(commandType, list);
        }
        else
        {
            pipeline.Add(commandPreprocessor);
        }
    }
    
    public void AddPostprocessorForCommand<TCommand>(Type commandPreprocessor) where TCommand : ICommand
    {
        var commandType = typeof(TCommand);
        _commandPostprocessors.TryGetValue(commandType, out var pipeline);

        if (pipeline is null)
        {
            List<Type> list = new() { commandPreprocessor };
            _commandPostprocessors.TryAdd(commandType, list);
        }
        else
        {
            pipeline.Add(commandPreprocessor);
        }
    }
    
    public IReadOnlyCollection<Type> GetPreprocessors<TCommand>(TCommand command)
        where TCommand : ICommand
    {
        var commandType = command.GetType();

        var source = _commandPreprocessors.ContainsKey(commandType)
            ? _commandPreprocessors[commandType]
            : _globalPreprocessors;
        return source.ToArray();
    }

    public IReadOnlyCollection<Type> GetMiddlewares<TCommand>(TCommand command)
        where TCommand : ICommand
    {
        var commandType = command.GetType();
        var source = _commandMiddlewares.ContainsKey(commandType)
            ? _commandMiddlewares[commandType]
            : _globalMiddlewares;

        return source.ToArray();
    }

    public IReadOnlyCollection<Type> GetPostprocessors<TCommand>(TCommand command)
        where TCommand : ICommand
    {
        var commandType = command.GetType();
        
        var source = _commandPostprocessors.ContainsKey(commandType)
            ? _commandPostprocessors[commandType]
            : _globalPostprocessor;

        return source.ToArray();
    }
}