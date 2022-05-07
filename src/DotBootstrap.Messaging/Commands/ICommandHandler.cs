using DotBootstrap.Messaging.Contracts;

namespace DotBootstrap.Messaging.Commands;

public interface ICommandHandler<in TRequest> where TRequest : ICommand
{
    Task Execute(TRequest command);
}