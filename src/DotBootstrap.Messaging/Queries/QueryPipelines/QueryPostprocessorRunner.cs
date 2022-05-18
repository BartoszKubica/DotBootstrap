using DotBootstrap.Messaging.Contracts;

namespace DotBootstrap.Messaging.Queries.QueryPipelines;

internal interface IQueryPostprocessorRunner
{
    Task<TResponse> Process<TQuery, TResponse>(TQuery query, TResponse result,
        CancellationToken cancellationToken) 
        where TQuery : IQuery<TResponse>;
}

internal class QueryPostprocessorRunner : IQueryPostprocessorRunner
{
    private readonly IQueryPipelineInvoker _queryPipelineInvoker;

    public QueryPostprocessorRunner(IQueryPipelineInvoker queryPipelineInvoker)
    {
        _queryPipelineInvoker = queryPipelineInvoker;
    }

    public async Task<TResponse> Process<TQuery, TResponse>(TQuery query, TResponse result,
        CancellationToken cancellationToken) 
        where TQuery : IQuery<TResponse>
    {
        var postprocessors = _queryPipelineInvoker
            .InvokePostprocessors<TQuery, TResponse>(query);

        return await postprocessors.Aggregate(Task.FromResult(result), async (current, postprocessor) => 
            await postprocessor.Process(query, await current, cancellationToken));
    }
}

