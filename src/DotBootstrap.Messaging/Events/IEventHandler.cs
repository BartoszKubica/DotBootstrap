using DotBootstrap.Messaging.Contracts;

namespace DotBootstrap.Messaging.Events;

public interface IEventHandler<in TEvent> 
    where TEvent : IEvent
{
    Task Process(TEvent @event);
}