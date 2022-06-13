using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace DotBootstrap.Persistence.IntegrationTests.Repositories;

public class AggregateRepositoryDecoratorTests : IClassFixture<PersistenceFixture>
{
    private readonly PersistenceFixture _fixture;

    public AggregateRepositoryDecoratorTests(PersistenceFixture fixture)
    {
        _fixture = fixture;
    }


    [Fact]
    public async Task Decorate_ShouldInvokeDecoratorAndDecoratedRepository()
    {
        var aggregate = new TestAggregate(Guid.NewGuid(), 0, "test", 1);
        await _fixture.Repository.Add(aggregate, CancellationToken.None);
        
        var savedCount = await _fixture.DbContext.SaveChangesAsync();
        var result = await _fixture.Repository.Get(aggregate.Id);

        savedCount.Should().Be(1);
        result.Should().BeEquivalentTo(aggregate);
        _fixture.InvokeRecorder.Messages.First().Should().Be("decoratedAdd");
    }
}