using DotBootstrap.Messaging.Contracts;
using DotBootstrap.Messaging.Queries.QueryPipelines;

namespace DotBootstrap.Messaging.Queries;

internal interface IQueryPipelineRunner
{
    Task<TResponse> RunPipeline<TQuery, TResponse>(TQuery query, CancellationToken cancellationToken,
        Func<Task<TResponse>> queryWrapper) where TQuery : IQuery<TResponse>;
}

internal class QueryPipelineRunner : IQueryPipelineRunner
{
    private readonly IQueryPostprocessorRunner _queryPostprocessorRunner;
    private readonly IQueryPreprocessorRunner _queryPreprocessorRunner;
    private readonly IQueryPipelineInvoker _queryPipelineInvoker;

    public QueryPipelineRunner(IQueryPostprocessorRunner queryPostprocessorRunner, IQueryPreprocessorRunner queryPreprocessorRunner, IQueryPipelineInvoker queryPipelineInvoker)
    {
        _queryPostprocessorRunner = queryPostprocessorRunner;
        _queryPreprocessorRunner = queryPreprocessorRunner;
        _queryPipelineInvoker = queryPipelineInvoker;
    }

    public async Task<TResponse> RunPipeline<TQuery, TResponse>(TQuery query, CancellationToken cancellationToken,
        Func<Task<TResponse>> handlerExecution) where TQuery : IQuery<TResponse>
    {
        await _queryPreprocessorRunner.Process<TQuery, TResponse>(query, cancellationToken);

        var result = await _queryPipelineInvoker.InvokeMiddlewares<TQuery, TResponse>(query)
            .Reverse().Aggregate(handlerExecution, (next, middleware) =>
                () => middleware.Process(query, cancellationToken, next))();

        return await _queryPostprocessorRunner.Process(query, result, cancellationToken);
    }
}