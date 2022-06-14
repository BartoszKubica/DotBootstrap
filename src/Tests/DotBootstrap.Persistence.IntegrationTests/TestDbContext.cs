using System;
using System.Threading;
using System.Threading.Tasks;
using DotBootstrap.Domain;
using DotBootstrap.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DotBootstrap.Persistence.IntegrationTests;

internal class TestDbContext : DbContext
{
    public TestDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TestAggregateDb>(x =>
            {
                x.Property(p => p.Version).IsConcurrencyToken();
                x.HasKey(k => k.Id);
                x.Property(k => k.Id).ValueGeneratedNever();
                x.ToTable("TestAggregate");
            });
        base.OnModelCreating(modelBuilder);
    }
}

public class TestAggregate : Aggregate
{
    public string Text { get; set; }
    public int Number { get; set; }
    
    public TestAggregate(Guid id, long version, string text, int number) : base(id)
    {
        Version = version;
        Text = text;
        Number = number;
    }
}

internal class TestAggregateDb : IVersionedEntity
{
    public Guid Id { get; }
    public long Version { get; }
    public string Text { get; set; }
    public int Number { get; set; }

    public TestAggregateDb(Guid id, long version, string text, int number)
    {
        Id = id;
        Version = version;
        Text = text;
        Number = number;
    }
    
    private TestAggregateDb()
    {
        
    }
}

internal class TestDomainMapper : IDomainMapper<TestAggregateDb, TestAggregate>
{
    public TestAggregateDb Map(TestAggregate entity)
    {
        return new TestAggregateDb(entity.Id, entity.Version, entity.Text, entity.Number);
    }

    public TestAggregate Map(TestAggregateDb model)
    {
        return new TestAggregate(model.Id, model.Version, model.Text, model.Number);
    }
}
