using DotBootstrap.Messaging.Contracts;

namespace DotBootstrap.Messaging.Queries;

public interface IQueryPostprocessor<in TQuery, TResponse> where TQuery : IQuery<TResponse>
{
    public Task<TResponse> Process(TQuery query, TResponse result, CancellationToken cancellationToken);
}
