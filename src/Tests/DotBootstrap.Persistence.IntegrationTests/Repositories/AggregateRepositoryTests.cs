using System;
using System.Threading;
using System.Threading.Tasks;
using DotBootstrap.Messaging;
using DotBootstrap.Persistence.Exceptions;
using DotBootstrap.Persistence.Extensions;
using DotBootstrap.Persistence.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DotBootstrap.Persistence.IntegrationTests.Repositories;

public class AggregateRepositoryTests
{

    private readonly DbContext _dbContext;
    private readonly IRepository<TestAggregate> _repository;

    public AggregateRepositoryTests()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.ConfigureTestDbContext();
        serviceCollection.RegisterAggregateRepository<TestAggregateDb, TestAggregate, TestDomainMapper>();
        serviceCollection.AddMessaging();
        var sp = serviceCollection.BuildServiceProvider();
        _dbContext = sp.GetRequiredService<DbContext>();
        _dbContext.Database.Migrate();
        _repository = sp.GetRequiredService<IRepository<TestAggregate>>();
    }
    [Fact]
    public async Task Add_ShouldSaveEntityToDatabase()
    {
        var aggregate = new TestAggregate(Guid.NewGuid(), 0, "test", 1);
        await _repository.Add(aggregate, CancellationToken.None);
        
        var savedCount = await _dbContext.SaveChangesAsync();
        var result = await _repository.Get(aggregate.Id);

        savedCount.Should().Be(1);
        result.Should().BeEquivalentTo(aggregate);
    }
    
    [Fact]
    public async Task Delete_ShouldDeleteEntityFromDatabase()
    {
        var aggregate = new TestAggregate(Guid.NewGuid(), 0, "test", 1);
        await _repository.Add(aggregate, CancellationToken.None);
        await _dbContext.SaveChangesAsync();
        var result = await _repository.Get(aggregate.Id);

        await _repository.Delete(result, result.Version);
        await _dbContext.SaveChangesAsync();

        var deletedEntity = await _dbContext.Set<TestAggregateDb>()
            .SingleOrDefaultAsync(x => x.Id == aggregate.Id);

        deletedEntity.Should().BeNull();
    }
    
    [Fact]
    public async Task Update_ShouldUpdateEntityInDatabase()
    {
        var aggregate = new TestAggregate(Guid.NewGuid(), 0, "test", 1);
        await _repository.Add(aggregate, CancellationToken.None);
        await _dbContext.SaveChangesAsync();
        var result = await _repository.Get(aggregate.Id);

        result.Number = 3;
        result.Text = "test2";
        await _repository.Update(result, result.Version);
        await _dbContext.SaveChangesAsync();

        var updatedEntity = await _dbContext.Set<TestAggregateDb>()
            .SingleAsync(x => x.Id == aggregate.Id);

        updatedEntity.Number.Should().Be(3);
        updatedEntity.Text.Should().Be("test2");
    }
    
    [Fact]
    public async Task Update_WhenVersionIsDifferent_ShouldThrowOptimisticConcurrencyException()
    {
        var aggregate = new TestAggregate(Guid.NewGuid(), 0, "test", 1);
        await _repository.Add(aggregate, CancellationToken.None);
        await _dbContext.SaveChangesAsync();
        var result = await _repository.Get(aggregate.Id);

        result.Number = 3;
        result.Text = "test2";
        var act = async () => await _repository.Update(result, 3);

        await act.Should().ThrowAsync<OptimisticConcurrencyException>();
    }
}