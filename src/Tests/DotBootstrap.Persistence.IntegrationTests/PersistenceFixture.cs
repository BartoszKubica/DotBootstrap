using System;
using System.Linq;
using DotBootstrap.Domain;
using DotBootstrap.Domain.DataPartitioning;
using DotBootstrap.Messaging;
using DotBootstrap.Persistence.Extensions;
using DotBootstrap.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DotBootstrap.Persistence.IntegrationTests;

public class PersistenceFixture : IDisposable
{
    public readonly DbContext DbContext;
    public readonly IRepository<TestAggregate> TestAggregateRepository;
    public readonly IRepository<TestTenantAggregate> TestTenantAggregateRepository;
    public readonly ITenantSetter TenantSetter;
    
    public PersistenceFixture()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.ConfigureTestDbContext();
        serviceCollection.AddDotBootstrap();
        serviceCollection.RegisterAggregateRepository<TestAggregateDb, TestAggregate, TestDomainMapper>();
        serviceCollection
            .RegisterAggregateRepository<TestTenantAggregateDb, TestTenantAggregate, TestTenantDomainMapper>(c =>
                c.WithTenantGuardDecorator());
        
        
        serviceCollection.AddMessaging();
        
        var sp = serviceCollection.BuildServiceProvider();
        DbContext = sp.GetRequiredService<DbContext>();
        TenantSetter = sp.GetRequiredService<ITenantSetter>();
        
        DbContext.Database.Migrate();
        TestAggregateRepository = sp.GetRequiredService<IRepository<TestAggregate>>();
        TestTenantAggregateRepository = sp.GetRequiredService<IRepository<TestTenantAggregate>>();
    }

    public void Dispose()
    {
       // DbContext.Database.EnsureDeleted();
        DbContext?.Dispose();
    }
}
[CollectionDefinition("PersistenceFixture")]
public class PersistenceFixtureCollection : ICollectionFixture<PersistenceFixture>
{
}