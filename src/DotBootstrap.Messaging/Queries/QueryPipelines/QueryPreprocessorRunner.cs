using DotBootstrap.Messaging.Contracts;

namespace DotBootstrap.Messaging.Queries.QueryPipelines;

internal interface IQueryPreprocessorRunner
{
    Task Process<TQuery, TResponse>(TQuery query,
        CancellationToken cancellationToken)
        where TQuery : IQuery<TResponse>;
}

internal class QueryPreprocessorRunner : IQueryPreprocessorRunner
{
    private readonly IQueryPipelineInvoker _queryPipelineInvoker;

    public QueryPreprocessorRunner(IQueryPipelineInvoker queryPipelineInvoker)
    {
        _queryPipelineInvoker = queryPipelineInvoker;
    }

    public async Task Process<TQuery, TResponse>(TQuery query,
        CancellationToken cancellationToken)
        where TQuery : IQuery<TResponse>
    {
        var preprocessors = _queryPipelineInvoker.
            InvokePreprocessors<TQuery, TResponse>(query);

        foreach (var preprocessor in preprocessors)
        {
            await preprocessor.Process(query, cancellationToken);
        }
    }
}