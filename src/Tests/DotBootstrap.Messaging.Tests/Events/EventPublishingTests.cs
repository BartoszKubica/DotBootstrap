using System.Threading;
using System.Threading.Tasks;
using DotBootstrap.Messaging.Events;
using DotBootstrap.Messaging.Queries;
using DotBootstrap.Messaging.Tests.Queries;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DotBootstrap.Messaging.Tests.Events;

public class EventPublishingTests
{
    [Fact]
    public async Task Publish_ShouldInvokeEventHandler()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddMessaging();
        serviceCollection.RegisterEventHandler<TestEvent, TestEventHandler>();
        var sp = serviceCollection.BuildServiceProvider();
        var bus = sp.GetRequiredService<IEventBus>();

        var @event = new TestEvent(false);
        await bus.Publish(@event);

        @event.Handled.Should().Be(true);
    }
}