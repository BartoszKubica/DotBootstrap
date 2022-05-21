using DotBootstrap.Messaging.Commands;
using DotBootstrap.Messaging.Contracts;
using DotBootstrap.Messaging.Queries;

namespace DotBootstrap.Messaging;

public interface ICommandQueryDispatcher
{
    Task Dispatch(ICommand command, CancellationToken cancellationToken);
    Task<TResponse> Dispatch<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken);
}

internal class CommandQueryDispatcher : ICommandQueryDispatcher
{
    private readonly ICommandBus _commandBus;
    private readonly IQueryBus _queryBus;

    public CommandQueryDispatcher(ICommandBus commandBus, IQueryBus queryBus)
    {
        _commandBus = commandBus;
        _queryBus = queryBus;
    }

    public async Task Dispatch(ICommand command, CancellationToken cancellationToken)
    {
        await _commandBus.Send(command, cancellationToken);
    }
    
    public async Task<TResponse> Dispatch<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken)
    {
        return await _queryBus.Send(query, cancellationToken);
    }
}