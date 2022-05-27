using DotBootstrap.Messaging.Contracts;

namespace DotBootstrap.Messaging.Events;

internal abstract class EventWrapperBase
{
    public abstract Task Process(IEvent @event);
}

internal class EventWrapper<TEvent> : EventWrapperBase where TEvent : IEvent
{
    private readonly IEventHandler<TEvent> _eventHandler;


    public EventWrapper(IEventHandler<TEvent> eventHandler)
    {
        _eventHandler = eventHandler;
    }

    public override Task Process(IEvent @event)
    {
        return _eventHandler.Process((TEvent)@event);
    }
}