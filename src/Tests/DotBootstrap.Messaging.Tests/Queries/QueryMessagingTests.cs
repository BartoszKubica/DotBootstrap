using System.Threading;
using System.Threading.Tasks;
using DotBootstrap.Messaging.Commands;
using DotBootstrap.Messaging.Queries;
using DotBootstrap.Messaging.Tests.Commands;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DotBootstrap.Messaging.Tests.Queries;

public class QueryMessagingTests
{
    [Fact]
    public async Task Send_ShouldInvokeQueryHandler()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddMessaging();
        serviceCollection.RegisterQueryHandler<TestQuery, TestResponse, TestQueryHandler>();
        var invokerRecorder = new InvokeRecorder();
        serviceCollection.AddSingleton(invokerRecorder);
        var sp = serviceCollection.BuildServiceProvider();
        var bus = sp.GetRequiredService<IQueryBus>();

        var command = new TestQuery();
        var response = await bus.Send(command, CancellationToken.None);

        response.Handled.Should().BeTrue();
    }
}