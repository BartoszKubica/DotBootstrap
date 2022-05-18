using DotBootstrap.Messaging.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace DotBootstrap.Messaging.Commands.CommandPipelines;

internal interface ICommandPipelineInvoker
{
    IReadOnlyCollection<ICommandPreprocessor<TCommand>> InvokePreprocessors<TCommand>(TCommand command)
        where TCommand : ICommand;

    IReadOnlyCollection<ICommandPostprocessor<TCommand>> InvokePostprocessors<TCommand>(TCommand command)
        where TCommand : ICommand;

    IReadOnlyCollection<ICommandMiddleware<TCommand>> InvokeMiddlewares<TCommand>(TCommand command)
        where TCommand : ICommand;
}

internal class CommandPipelineInvoker : ICommandPipelineInvoker
{
    private readonly CommandPipelineStore _commandPipelineStore;
    private readonly IServiceProvider _serviceProvider;

    public CommandPipelineInvoker(CommandPipelineStore commandPipelineStore, IServiceProvider serviceProvider)
    {
        _commandPipelineStore = commandPipelineStore;
        _serviceProvider = serviceProvider;
    }

    public IReadOnlyCollection<ICommandPreprocessor<TCommand>> InvokePreprocessors<TCommand>(TCommand command)
        where TCommand : ICommand
    {
        return _commandPipelineStore.GetPreprocessors(command)
            .Select(type => type.MakeGenericType(command.GetType()))
            .Select(type => _serviceProvider.GetRequiredService(type))
            .Cast<ICommandPreprocessor<TCommand>>()
            .ToArray();
    }
    
    public IReadOnlyCollection<ICommandPostprocessor<TCommand>> InvokePostprocessors<TCommand>(TCommand command)
        where TCommand : ICommand
    {
        return _commandPipelineStore.GetPostprocessors(command)
            .Select(type => type.MakeGenericType(command.GetType()))
            .Select(type => _serviceProvider.GetRequiredService(type))
            .Cast<ICommandPostprocessor<TCommand>>()
            .ToArray();
    }
    
    public IReadOnlyCollection<ICommandMiddleware<TCommand>> InvokeMiddlewares<TCommand>(TCommand command)
        where TCommand : ICommand
    {
        return _commandPipelineStore.GetMiddlewares(command)
            .Select(type => type.MakeGenericType(command.GetType()))
            .Select(type => _serviceProvider.GetRequiredService(type))
            .Cast<ICommandMiddleware<TCommand>>()
            .ToArray();
    }
}