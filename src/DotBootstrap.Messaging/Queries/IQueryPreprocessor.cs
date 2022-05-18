using DotBootstrap.Messaging.Contracts;

namespace DotBootstrap.Messaging.Queries;

public interface IQueryPreprocessor<in TQuery, in TResponse> where TQuery : IQuery<TResponse>
{
    public Task Process(TQuery query, CancellationToken cancellationToken);
}
