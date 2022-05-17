using DotBootstrap.Messaging.Contracts;

namespace DotBootstrap.Messaging.Queries.QueryPipelines;

public interface IQueryMiddleware<in TQuery, TResponse> where TQuery : IQuery<TResponse>
{
    public Task<TResponse> Process(TQuery query, CancellationToken cancellationToken, Func<Task<TResponse>> next);
}

