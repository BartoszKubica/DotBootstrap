using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DotBootstrap.Messaging;
using DotBootstrap.Persistence.Extensions;
using DotBootstrap.Persistence.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DotBootstrap.Persistence.IntegrationTests.Repositories;

public class AggregateRepositoryDecoratorTests
{
    private readonly DbContext _dbContext;
    private readonly IRepository<TestAggregate> _repository;
    private readonly InvokeRecorder _invokeRecorder;
    public AggregateRepositoryDecoratorTests()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.ConfigureTestDbContext();
        serviceCollection.AddSingleton<InvokeRecorder>();
        serviceCollection.RegisterAggregateRepository<TestAggregateDb, TestAggregate, TestDomainMapper>(x
            => x.WithDecorator(typeof(RepositoryDecorator<>)));
        serviceCollection.AddMessaging();
        var sp = serviceCollection.BuildServiceProvider();
        _dbContext = sp.GetRequiredService<DbContext>();
        _invokeRecorder = sp.GetRequiredService<InvokeRecorder>();
        _dbContext.Database.Migrate();
        _repository = sp.GetRequiredService<IRepository<TestAggregate>>();
    }

    [Fact]
    public async Task Decorate_ShouldInvokeDecoratorAndDecoratedRepository()
    {
        var aggregate = new TestAggregate(Guid.NewGuid(), 0, "test", 1);
        await _repository.Add(aggregate, CancellationToken.None);
        
        var savedCount = await _dbContext.SaveChangesAsync();
        var result = await _repository.Get(aggregate.Id);

        savedCount.Should().Be(1);
        result.Should().BeEquivalentTo(aggregate);
        _invokeRecorder.Messages.First().Should().Be("decoratedAdd");
    }
}