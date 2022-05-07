using DotBootstrap.Messaging.Contracts;

namespace DotBootstrap.Messaging.Commands;

public interface ICommandPostprocessor<in TCommand> where TCommand : ICommand
{
    public Task Process(TCommand command);
}