using System.Threading.Tasks;
using DotBootstrap.Messaging.Contracts;
using DotBootstrap.Messaging.Events;

namespace DotBootstrap.Messaging.Tests.Events;

public record TestEvent(bool Handled) : IEvent
{
    public bool Handled { get; set; } = Handled;
}

public class TestEventHandler : IEventHandler<TestEvent>
{
    public Task Process(TestEvent @event)
    {
        @event.Handled = true;
        return Task.CompletedTask;
    }
}