using DotBootstrap.Messaging.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace DotBootstrap.Messaging.Events;

public interface IEventBus
{
    Task Publish(IEvent @event);
}

internal class EventBus : IEventBus
{
    private readonly IServiceProvider _serviceProvider;

    public EventBus(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task Publish(IEvent @event)
    {
        var eventType = @event.GetType();
        var genericHandlerType = typeof(IEventHandler<>).MakeGenericType(eventType);
        var eventHandler = typeof(EventWrapper<>).MakeGenericType(eventType);

        var handlers = _serviceProvider.GetServices(genericHandlerType)
            .Select(x => Activator.CreateInstance(eventHandler, x))
            .Cast<EventWrapperBase>()
            .ToList();

        foreach (var handler in handlers)
        {
            await handler.Process(@event);
        }
    }
}