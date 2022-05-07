using DotBootstrap.Messaging.Contracts;

namespace DotBootstrap.Messaging.Commands;

public interface ICommandPreprocessor<in TCommand> where TCommand : ICommand
{
    public Task Process(TCommand command);
}