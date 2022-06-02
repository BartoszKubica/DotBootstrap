using System;
using System.Linq;
using DotBootstrap.Messaging.Contracts;
using FluentAssertions;
using Xunit;

namespace DotBootstrap.Domain.Tests;

public class AggregateTests
{
    [Fact]
    public void Publish_ShouldAddEventToQueueAndSetVersion()
    {
        var aggregate = new TestAggregate(Guid.NewGuid());
        aggregate.Test();
        var @event = aggregate.Events.First();
        
        aggregate.Version.Should().Be(1);
        @event.Version.Should().Be(1);
    }
    
    [Fact]
    public void DequeueAllEvents_ShouldClearQueueAndReturnAllEvents()
    {
        var aggregate = new TestAggregate(Guid.NewGuid());
        aggregate.Test();

        var result = aggregate.DequeueAllEvents();

        result.Should().HaveCount(1);
        aggregate.Events.Should().BeEmpty();
    }


    private record TestAggregateEvent() : IAggregateEvent
    {
        public long Version { get; set; }
    }

    internal class TestAggregate : Aggregate
    {
        public TestAggregate(Guid id) : base(id)
        {
        }

        public void Test()
        {
            PublishEvent(new TestAggregateEvent());
        }
        
        
    }
}