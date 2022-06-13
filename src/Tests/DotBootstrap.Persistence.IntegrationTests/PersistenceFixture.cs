using System.Linq;
using DotBootstrap.Messaging;
using DotBootstrap.Persistence.Extensions;
using DotBootstrap.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DotBootstrap.Persistence.IntegrationTests;

public class PersistenceFixture
{
    public readonly DbContext DbContext;
    public readonly IRepository<TestAggregate> Repository;
    public readonly InvokeRecorder InvokeRecorder;

    public PersistenceFixture()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.ConfigureTestDbContext();
        serviceCollection.AddSingleton<InvokeRecorder>();
        serviceCollection.RegisterAggregateRepository<TestAggregateDb, TestAggregate, TestDomainMapper>(x
            => x.WithDecorator(typeof(RepositoryDecorator<>)));
        serviceCollection.AddMessaging();
        var sp = serviceCollection.BuildServiceProvider();
        DbContext = sp.GetRequiredService<DbContext>();
        InvokeRecorder = sp.GetRequiredService<InvokeRecorder>();
        
        if(this.DbContext.Database.GetPendingMigrations().Any())
            DbContext.Database.Migrate();
        Repository = sp.GetRequiredService<IRepository<TestAggregate>>();
    }
}