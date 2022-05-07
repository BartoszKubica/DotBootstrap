using DotBootstrap.Messaging.Contracts;

namespace DotBootstrap.Messaging.Commands.CommandPipelines;

public interface ICommandMiddleware<in TCommand> where TCommand : ICommand
{
    public Task Process(TCommand command, Func<Task> next);
}