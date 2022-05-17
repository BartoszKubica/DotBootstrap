using DotBootstrap.Messaging.Contracts;

namespace DotBootstrap.Messaging.Queries;

public interface IQueryHandler<in TQuery, TResponse> where TQuery : IQuery<TResponse>
{
    Task<TResponse> Execute(TQuery query, CancellationToken cancellationToken);
}