using DotBootstrap.Messaging.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace DotBootstrap.Messaging.Commands;

public interface ICommandBus
{
    Task Send<TCommand>(TCommand command) where TCommand : ICommand;
}

internal class CommandBus : ICommandBus
{
    private readonly ICommandPipelineRunner _pipelineRunner;
    private readonly IServiceProvider _serviceProvider;

    public CommandBus(IServiceProvider serviceProvider, ICommandPipelineRunner pipelineRunner)
    {
        _serviceProvider = serviceProvider;
        _pipelineRunner = pipelineRunner;
    }

    public Task Send<TCommand>(TCommand command) where TCommand : ICommand
    {
        var commandType = command.GetType();
        var genericHandlerType = typeof(ICommandHandler<>).MakeGenericType(commandType);
        var commandWrapperType = typeof(CommandWrapper<>).MakeGenericType(commandType);

        var handler = _serviceProvider.GetRequiredService(genericHandlerType);

        var wrapper = (CommandWrapperBase?)Activator.CreateInstance(commandWrapperType, handler)
            ?? throw new NullReferenceException("Could not create command handler");

        return _pipelineRunner.RunPipeline(command, wrapper);
    }
}