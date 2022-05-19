using System;
using System.Threading;
using System.Threading.Tasks;
using DotBootstrap.Messaging.Commands;
using DotBootstrap.Messaging.Queries;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DotBootstrap.Messaging.Tests.Queries;

public class QueryMiddlewaresTests
{
    [Fact]
    public async Task Send_WithGlobalMiddlewaresSet_ShouldInvokeMiddlewares()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddMessaging(queryConfiguration: q =>
            q.AddGlobalMiddleware(typeof(TestQueryMiddleware<,>))
                .AddGlobalMiddleware(typeof(TestQueryMiddleware2<,>)));
        serviceCollection.RegisterQueryHandler<TestQuery, TestResponse, TestQueryHandler>();
        var invokerRecorder = new InvokeRecorder();
        serviceCollection.AddSingleton(invokerRecorder);
        var sp = serviceCollection.BuildServiceProvider();
        var bus = sp.GetRequiredService<IQueryBus>();

        var command = new TestQuery();
        var response = await bus.Send(command, CancellationToken.None);


        invokerRecorder.Messages[0].Should().Be($"{nameof(TestQueryMiddleware<TestQuery, TestResponse>)} before");
        invokerRecorder.Messages[1].Should().Be($"{nameof(TestQueryMiddleware2<TestQuery, TestResponse>)} before");
        invokerRecorder.Messages[2].Should().Be($"{command.GetType()} {nameof(TestQueryHandler)}");
        invokerRecorder.Messages[3].Should().Be($"{nameof(TestQueryMiddleware2<TestQuery, TestResponse>)} after");
        invokerRecorder.Messages[4].Should().Be($"{nameof(TestQueryMiddleware<TestQuery, TestResponse>)} after");
        
        
        response.Handled.Should().BeTrue();
    }

    [Fact]
    public async Task Send_WithCommandMiddlewareSet_ShouldInvokeOnlySetMiddleware()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddMessaging(queryConfiguration: q =>
            q.AddMiddlewareForQuery<TestQuery, TestResponse>(typeof(TestQueryMiddleware<,>))
                .AddGlobalMiddleware(typeof(TestQueryMiddleware2<,>)));
        serviceCollection.RegisterQueryHandler<TestQuery, TestResponse, TestQueryHandler>();
        var invokerRecorder = new InvokeRecorder();
        serviceCollection.AddSingleton(invokerRecorder);
        var sp = serviceCollection.BuildServiceProvider();
        var bus = sp.GetRequiredService<IQueryBus>();

        var command = new TestQuery();
        var response = await bus.Send(command, CancellationToken.None);

        invokerRecorder.Messages[0].Should().Be($"{nameof(TestQueryMiddleware<TestQuery, TestResponse>)} before");
        invokerRecorder.Messages[1].Should().Be($"{command.GetType()} {nameof(TestQueryHandler)}");
        invokerRecorder.Messages[2].Should().Be($"{nameof(TestQueryMiddleware<TestQuery, TestResponse>)} after");
        response.Handled.Should().BeTrue();
    }
    
    [Fact]
    public void AddNotMiddlewareType_AsMiddleware_ShouldThrowException()
    {
        var serviceCollection = new ServiceCollection();
        var act = () => serviceCollection.AddMessaging(queryConfiguration: q =>
            q.AddGlobalMiddleware(typeof(TestQuery)));

        act.Should().ThrowExactly<ArgumentException>();
    }
}