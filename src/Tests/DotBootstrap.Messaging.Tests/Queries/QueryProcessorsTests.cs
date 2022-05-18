using System;
using System.Threading;
using System.Threading.Tasks;
using DotBootstrap.Messaging.Commands;
using DotBootstrap.Messaging.Queries;
using DotBootstrap.Messaging.Tests.Commands;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DotBootstrap.Messaging.Tests.Queries;

public class QueryProcessorsTests
{
    [Fact]
    public async Task Send_WithGlobalPostprocessorsSet_ShouldInvokePostprocessor()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddMessaging(queryConfiguration: q =>
            q.AddGlobalPostprocessor(typeof(TestQueryPostprocessor<,>)));
        serviceCollection.RegisterQueryHandler<TestQuery, TestResponse, TestQueryHandler>();
        var invokerRecorder = new InvokeRecorder();
        serviceCollection.AddSingleton(invokerRecorder);
        var sp = serviceCollection.BuildServiceProvider();
        var bus = sp.GetRequiredService<IQueryBus>();

        var query = new TestQuery();
        var response = await bus.Send(query, CancellationToken.None);

        invokerRecorder.Messages[0].Should().Be($"{query.GetType()} {nameof(TestQueryHandler)}");
        invokerRecorder.Messages[1].Should()
            .Be($"{query.GetType()} {nameof(TestQueryPostprocessor<TestQuery, TestResponse>)}");

        response.Handled.Should().BeTrue();
    }

    [Fact]
    public async Task Send_WithQueryPostprocessorsSet_ShouldInvokePostprocessorOnlyForGivenQuery()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddMessaging(queryConfiguration: q =>
            q.AddPostprocessorForQuery<TestQuery, TestResponse>(typeof(TestQueryPostprocessor<,>)));

        serviceCollection.RegisterQueryHandler<TestQuery, TestResponse, TestQueryHandler>();;
        serviceCollection.RegisterQueryHandler<TestQuery2, TestResponse, TestQueryHandler2>();

        var invokerRecorder = new InvokeRecorder();
        serviceCollection.AddSingleton(invokerRecorder);
        var sp = serviceCollection.BuildServiceProvider();

        var queryBus = sp.GetRequiredService<IQueryBus>();

        var query = new TestQuery();
        var query1 = new TestQuery2();
        var result1 = await queryBus.Send(query, CancellationToken.None);
        var result2 = await queryBus.Send(query1, CancellationToken.None);

        invokerRecorder.Messages[0].Should().Be($"{query.GetType()} {nameof(TestQueryHandler)}");
        invokerRecorder.Messages[1].Should().Be($"{query.GetType()} {nameof(TestQueryPostprocessor<TestQuery, TestResponse>)}");

        result1.Handled.Should().BeTrue();
        result2.Handled.Should().BeTrue();
    }
    
    [Fact]
    public async Task Send_WithGlobalPreprocessorsSet_ShouldInvokePreprocessor()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddMessaging(queryConfiguration: q =>
            q.AddGlobalPreprocessor(typeof(TestQueryPreprocessor<,>)));
        serviceCollection.RegisterQueryHandler<TestQuery, TestResponse, TestQueryHandler>();
        var invokerRecorder = new InvokeRecorder();
        serviceCollection.AddSingleton(invokerRecorder);
        var sp = serviceCollection.BuildServiceProvider();
        var bus = sp.GetRequiredService<IQueryBus>();

        var query = new TestQuery();
        var response = await bus.Send(query, CancellationToken.None);

        invokerRecorder.Messages[0].Should()
            .Be($"{query.GetType()} {nameof(TestQueryPreprocessor<TestQuery, TestResponse>)}");
        invokerRecorder.Messages[1].Should().Be($"{query.GetType()} {nameof(TestQueryHandler)}");
       
        response.Handled.Should().BeTrue();
    }

    [Fact]
    public async Task Send_WithQueryPreprocessorsSet_ShouldInvokePreprocessorOnlyForGivenQuery()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddMessaging(queryConfiguration: q =>
            q.AddPreprocessorForQuery<TestQuery, TestResponse>(typeof(TestQueryPreprocessor<,>)));

        serviceCollection.RegisterQueryHandler<TestQuery, TestResponse, TestQueryHandler>();;
        serviceCollection.RegisterQueryHandler<TestQuery2, TestResponse, TestQueryHandler2>();

        var invokerRecorder = new InvokeRecorder();
        serviceCollection.AddSingleton(invokerRecorder);
        var sp = serviceCollection.BuildServiceProvider();

        var queryBus = sp.GetRequiredService<IQueryBus>();

        var query = new TestQuery();
        var query1 = new TestQuery2();
        var result1 = await queryBus.Send(query, CancellationToken.None);
        var result2 = await queryBus.Send(query1, CancellationToken.None);

        invokerRecorder.Messages[0].Should().Be($"{query.GetType()} {nameof(TestQueryPreprocessor<TestQuery, TestResponse>)}");
        invokerRecorder.Messages[1].Should().Be($"{query.GetType()} {nameof(TestQueryHandler)}");

        result1.Handled.Should().BeTrue();
        result2.Handled.Should().BeTrue();
    }
    
    [Fact]
    public void AddNotPreprocessorType_AsPreprocessor_ShouldThrowException()
    {
        var serviceCollection = new ServiceCollection();
        var act = () => serviceCollection.AddMessaging(queryConfiguration: q =>
            q.AddGlobalPreprocessor(typeof(TestQuery)));

        act.Should().ThrowExactly<ArgumentException>();
    }

    [Fact]
    public void AddNotPostprocessorType_AsPostprocessor_ShouldThrowException()
    {
        var serviceCollection = new ServiceCollection();
        var act = () => serviceCollection.AddMessaging(queryConfiguration: q =>
            q.AddGlobalPostprocessor(typeof(TestQuery)));

        act.Should().ThrowExactly<ArgumentException>();
    }
}