using System;
using System.Threading;
using System.Threading.Tasks;
using DotBootstrap.Persistence.Exceptions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DotBootstrap.Persistence.IntegrationTests.Repositories;

[Collection("PersistenceFixture")]
public class AggregateRepositoryTests
{
    private readonly PersistenceFixture _fixture;

    public AggregateRepositoryTests(PersistenceFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Add_ShouldSaveEntityToDatabase()
    {
        var aggregate = new TestAggregate(Guid.NewGuid(), 0, "test", 1);
        await _fixture.Repository.Add(aggregate, CancellationToken.None);
        
        var savedCount = await _fixture.DbContext.SaveChangesAsync();
        var result = await _fixture.Repository.Get(aggregate.Id);

        savedCount.Should().Be(1);
        result.Should().BeEquivalentTo(aggregate);
    }
    
    [Fact]
    public async Task Delete_ShouldDeleteEntityFromDatabase()
    {
        var aggregate = new TestAggregate(Guid.NewGuid(), 0, "test", 1);
        await _fixture.Repository.Add(aggregate, CancellationToken.None);
        await _fixture.DbContext.SaveChangesAsync();
        var result = await _fixture.Repository.Get(aggregate.Id);

        await _fixture.Repository.Delete(result, result.Version);
        await _fixture.DbContext.SaveChangesAsync();

        var deletedEntity = await _fixture.DbContext.Set<TestAggregateDb>()
            .SingleOrDefaultAsync(x => x.Id == aggregate.Id);

        deletedEntity.Should().BeNull();
    }
    
    [Fact]
    public async Task Update_ShouldUpdateEntityInDatabase()
    {
        var aggregate = new TestAggregate(Guid.NewGuid(), 0, "test", 1);
        await _fixture.Repository.Add(aggregate, CancellationToken.None);
        await _fixture.DbContext.SaveChangesAsync();
        var result = await _fixture.Repository.Get(aggregate.Id);

        result.Number = 3;
        result.Text = "test2";
        await _fixture.Repository.Update(result, result.Version);
        await _fixture.DbContext.SaveChangesAsync();

        var updatedEntity = await _fixture.DbContext.Set<TestAggregateDb>()
            .SingleAsync(x => x.Id == aggregate.Id);

        updatedEntity.Number.Should().Be(3);
        updatedEntity.Text.Should().Be("test2");
    }
    
    [Fact]
    public async Task Update_WhenVersionIsDifferent_ShouldThrowOptimisticConcurrencyException()
    {
        var aggregate = new TestAggregate(Guid.NewGuid(), 0, "test", 1);
        await _fixture.Repository.Add(aggregate, CancellationToken.None);
        await _fixture.DbContext.SaveChangesAsync();
        var result = await _fixture.Repository.Get(aggregate.Id);

        result.Number = 3;
        result.Text = "test2";
        var act = async () => await _fixture.Repository.Update(result, 3);

        await act.Should().ThrowAsync<OptimisticConcurrencyException>();
    }
}