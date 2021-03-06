using System;
using DotBootstrap.Domain;
using DotBootstrap.Domain.DataPartitioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DotBootstrap.Persistence.IntegrationTests;

public class TestDbContext : DbContext
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
        
        modelBuilder.Entity<TestTenantAggregateDb>(x =>
        {
            x.Property(p => p.Version).IsConcurrencyToken();
            x.HasKey(k => k.Id);
            x.Property(k => k.Id).ValueGeneratedNever();
            x.Property(p => p.TenantId).ValueGeneratedNever().IsRequired();
            x.ToTable("TestTenantAggregate");
        });
        base.OnModelCreating(modelBuilder);
    }
}

public class TestDbContextFactory : IDesignTimeDbContextFactory<TestDbContext>
{
    public TestDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<TestDbContext>();
        optionsBuilder.UseNpgsql(DbContextProvider.ConnectionString);

        return new TestDbContext(optionsBuilder.Options);
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

public class TestTenantAggregate : Aggregate, ITenantEntity
{
    public string Text { get; set; }
    public int Number { get; set; }
    public Guid TenantId { get; }

    public TestTenantAggregate(Guid id, long version, string text, int number, Guid tenantId) : base(id)
    {
        Version = version;
        Text = text;
        Number = number;
        TenantId = tenantId;
    }

}

internal class TestTenantAggregateDb : IVersionedEntity, ITenantEntity
{
    public Guid Id { get; }
    public long Version { get; }
    public string Text { get; set; }
    public int Number { get; set; }
    public Guid TenantId { get; }

    public TestTenantAggregateDb(Guid id, long version, string text, int number, Guid tenantId)
    {
        Id = id;
        Version = version;
        Text = text;
        Number = number;
        TenantId = tenantId;
    }
    
    private TestTenantAggregateDb()
    {
    }

}
internal class TestTenantDomainMapper : IDomainMapper<TestTenantAggregateDb, TestTenantAggregate>
{
    public TestTenantAggregateDb Map(TestTenantAggregate entity)
    {
        return new TestTenantAggregateDb(entity.Id, entity.Version, entity.Text, entity.Number, entity.TenantId);
    }

    public TestTenantAggregate Map(TestTenantAggregateDb model)
    {
        return new TestTenantAggregate(model.Id, model.Version, model.Text, model.Number, model.TenantId);
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
