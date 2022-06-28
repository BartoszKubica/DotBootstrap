using System;
using System.Threading;
using System.Threading.Tasks;
using DotBootstrap.Persistence.Exceptions;
using FluentAssertions;
using Xunit;

namespace DotBootstrap.Persistence.IntegrationTests.Repositories;

[Collection("PersistenceFixture")]
public class TenantRepositoryDecoratorTests
{
    private readonly PersistenceFixture _fixture;

    public TenantRepositoryDecoratorTests(PersistenceFixture fixture)
    {
        _fixture = fixture;
    }
    
    [Fact]
    public async Task Add_WhenTenantIsDifferentThanGiven_ShouldThrowException()
    {
        var aggregate = new TestTenantAggregate(Guid.NewGuid(), 0, "test", 1, Guid.NewGuid());
        var act = () => _fixture.InvokeInTenantScope(async () =>
        {
            await _fixture.TestTenantAggregateRepository.Add(aggregate, CancellationToken.None);

        }, Guid.NewGuid());

        await act.Should().ThrowAsync<OutOfTenantException>();
    }
    
    [Fact]
    public async Task Delete_WhenTenantIsDifferentThanGiven_ShouldThrowException()
    {
        var aggregate = new TestTenantAggregate(Guid.NewGuid(), 0, "test", 1, Guid.NewGuid());
        var act = () => _fixture.InvokeInTenantScope(async () =>
        {
            await _fixture.TestTenantAggregateRepository.Delete(aggregate, aggregate.Version);

        }, Guid.NewGuid());

        await act.Should().ThrowAsync<OutOfTenantException>();
    }
    
    [Fact]
    public async Task Update_WhenTenantIsDifferentThanGiven_ShouldThrowException()
    {
        var aggregate = new TestTenantAggregate(Guid.NewGuid(), 0, "test", 1, Guid.NewGuid());
        
       await _fixture.InvokeInTenantScope(async () =>
        {
            await _fixture.TestTenantAggregateRepository.Add(aggregate, CancellationToken.None);
            await _fixture.DbContext.SaveChangesAsync();
        }, aggregate.TenantId);

       var act = () => _fixture.InvokeInTenantScope(async () =>
       {
           await _fixture.TestTenantAggregateRepository.Update(aggregate, aggregate.Version);
       }, Guid.NewGuid());
       
       await act.Should().ThrowAsync<OutOfTenantException>();
    }
}