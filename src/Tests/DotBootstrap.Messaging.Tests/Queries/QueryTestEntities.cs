using System;
using System.Threading;
using System.Threading.Tasks;
using DotBootstrap.Messaging.Contracts;
using DotBootstrap.Messaging.Queries;
using DotBootstrap.Messaging.Queries.QueryPipelines;

namespace DotBootstrap.Messaging.Tests.Queries;

public record TestQuery : IQuery<TestResponse>;

public record TestQuery2 : IQuery<TestResponse>;
public record TestResponse(bool Handled);

public class TestQueryHandler : IQueryHandler<TestQuery, TestResponse>
{
    private readonly InvokeRecorder _invokeRecorder;

    public TestQueryHandler(InvokeRecorder invokeRecorder)
    {
        _invokeRecorder = invokeRecorder;
    }

    public async Task<TestResponse> Execute(TestQuery query, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        _invokeRecorder.Messages.Add($"{query.GetType()} {nameof(TestQueryHandler)}");
        return new TestResponse(true);
    }
}

public class TestQueryHandler2 : IQueryHandler<TestQuery2, TestResponse>
{
    private readonly InvokeRecorder _invokeRecorder;

    public TestQueryHandler2(InvokeRecorder invokeRecorder)
    {
        _invokeRecorder = invokeRecorder;
    }

    public async Task<TestResponse> Execute(TestQuery2 query, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        _invokeRecorder.Messages.Add($"{query.GetType()} {nameof(TestQueryHandler2)}");
        return new TestResponse(true);
    }
}

public class TestQueryPostprocessor<TQuery, TResponse> : IQueryPostprocessor<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    private readonly InvokeRecorder _invokeRecorder;

    public TestQueryPostprocessor(InvokeRecorder invokeRecorder)
    {
        _invokeRecorder = invokeRecorder;
    }

    public Task<TResponse> Process(TQuery query, TResponse result, CancellationToken cancellationToken)
    {
        _invokeRecorder.Messages.Add($"{query.GetType()} {nameof(TestQueryPostprocessor<TQuery, TResponse>)}");

        return Task.FromResult(result);
    }
}


public class TestQueryPreprocessor<TQuery, TResponse> : IQueryPreprocessor<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    private readonly InvokeRecorder _invokeRecorder;

    public TestQueryPreprocessor(InvokeRecorder invokeRecorder)
    {
        _invokeRecorder = invokeRecorder;
    }

    public Task Process(TQuery query, CancellationToken cancellationToken)
    {
        _invokeRecorder.Messages.Add($"{query.GetType()} {nameof(TestQueryPreprocessor<TQuery, TResponse>)}");
        return Task.CompletedTask;
    }
}

public class TestQueryMiddleware<TQuery, TResponse> : IQueryMiddleware<TQuery, TResponse> where TQuery : IQuery<TResponse>
{
    private readonly InvokeRecorder _invokeRecorder;

    public TestQueryMiddleware(InvokeRecorder invokeRecorder)
    {
        _invokeRecorder = invokeRecorder;
    }


    public Task<TResponse> Process(TQuery query, CancellationToken cancellationToken, Func<Task<TResponse>> next)
    {
        _invokeRecorder.Messages.Add($"{nameof(TestQueryMiddleware<TQuery, TResponse>)} before");
        var result = next();
        _invokeRecorder.Messages.Add($"{nameof(TestQueryMiddleware<TQuery, TResponse>)} after");
        return result;
    }
}

public class TestQueryMiddleware2<TQuery, TResponse> : IQueryMiddleware<TQuery, TResponse> where TQuery : IQuery<TResponse>
{
    private readonly InvokeRecorder _invokeRecorder;

    public TestQueryMiddleware2(InvokeRecorder invokeRecorder)
    {
        _invokeRecorder = invokeRecorder;
    }

    public Task<TResponse> Process(TQuery query, CancellationToken cancellationToken, Func<Task<TResponse>> next)
    {
        _invokeRecorder.Messages.Add($"{nameof(TestQueryMiddleware2<TQuery, TResponse>)} before");
        var result = next();
        _invokeRecorder.Messages.Add($"{nameof(TestQueryMiddleware2<TQuery, TResponse>)} after");

        return result;
    }
}
