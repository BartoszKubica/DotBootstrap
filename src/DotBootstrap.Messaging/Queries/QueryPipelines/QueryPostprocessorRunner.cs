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
        //todo: find better solution
        var resultTemp = result;
        foreach (var postprocessor in postprocessors)
        { 
           resultTemp = await postprocessor.Process(query, resultTemp, cancellationToken);
        }

        return resultTemp;
    }
}