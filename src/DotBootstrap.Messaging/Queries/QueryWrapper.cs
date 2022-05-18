using DotBootstrap.Messaging.Contracts;

namespace DotBootstrap.Messaging.Queries;

internal abstract class QueryWrapperBase<TResponse>
{
    public abstract Task<TResponse> Process(IQuery<TResponse> query, CancellationToken cancellationToken);
}

internal class QueryWrapper<TQuery, TResponse> : QueryWrapperBase<TResponse> where TQuery : IQuery<TResponse>
{
    private readonly IQueryHandler<TQuery, TResponse> _queryHandler;
    private readonly IQueryPipelineRunner _queryPipelineRunner;

    public QueryWrapper(IQueryHandler<TQuery, TResponse> queryHandler, IQueryPipelineRunner queryPipelineRunner)
    {
        _queryHandler = queryHandler;
        _queryPipelineRunner = queryPipelineRunner;
    }

    public override Task<TResponse> Process(IQuery<TResponse> query, CancellationToken cancellationToken)
    {
        return _queryPipelineRunner.RunPipeline((TQuery)query, cancellationToken,
            () => _queryHandler.Execute((TQuery)query, cancellationToken));
    }
}