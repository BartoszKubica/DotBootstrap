using DotBootstrap.Messaging.Contracts;

namespace DotBootstrap.Domain;

public abstract class Aggregate : IVersionedEntity
{
    public Guid Id { get; }
    public long Version { get; private set; }
    private readonly Queue<IAggregateEvent> _eventQueue = new();
    public IReadOnlyCollection<IAggregateEvent> Events => _eventQueue;

    protected Aggregate(Guid id)
    {
        Id = id;
        Version = 0;
    }

    protected void PublishEvent(IAggregateEvent @event)
    {
        Version++;
        @event.Version = Version;
        _eventQueue.Enqueue(@event);
    }

    public IReadOnlyCollection<IAggregateEvent> DequeueAllEvents()
    {
        var events = _eventQueue.ToArray();
        _eventQueue.Clear();
        return events;
    }
}