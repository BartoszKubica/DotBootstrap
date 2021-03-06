using DotBootstrap.Messaging.Contracts;

namespace DotBootstrap.Messaging.Commands;

internal abstract class CommandWrapperBase
{
    public abstract Task Process(ICommand command, CancellationToken cancellationToken);
}

internal class CommandWrapper<TCommand> : CommandWrapperBase where TCommand : ICommand
{
    private readonly ICommandHandler<TCommand> _handler;

    public CommandWrapper(ICommandHandler<TCommand> handler)
    {
        _handler = handler;
    }

    public override async Task Process(ICommand command, CancellationToken cancellationToken)
    {
        await _handler.Execute((TCommand)command, cancellationToken);
    }
}