namespace DotBootstrap.Messaging.Contracts;

public interface IAggregateEvent : IEvent
{
    public long Version { get; set; }
}