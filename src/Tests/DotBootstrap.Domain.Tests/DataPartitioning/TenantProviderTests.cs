using System;
using DotBootstrap.Domain.DataPartitioning;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DotBootstrap.Domain.Tests.DataPartitioning;

public class TenantProviderTests
{
    private readonly ITenantContext _tenantContext;
    private readonly ITenantSetter _tenantSetter;
    
    public TenantProviderTests()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddDotBootstrap();

        var sp = serviceCollection.BuildServiceProvider();
        _tenantContext = sp.GetRequiredService<ITenantContext>();
        _tenantSetter = sp.GetRequiredService<ITenantSetter>();
    }

    [Fact]
    public void WhenSetTenantIdBySetter_TenantContextShouldReturnSameId()
    {
        var id = Guid.NewGuid();
        _tenantSetter.SetTenantId(id);

        _tenantContext.GetTenantId().Should().Be(id);
        _tenantContext.HasTenantId().Should().BeTrue();
    } 
    
    [Fact]
    public void WhenNoTenantSet_TenantContextShouldThrowException()
    {
        var act = () => _tenantContext.GetTenantId();

        act.Should().Throw<TenantContextIsNotSet>();
        _tenantContext.HasTenantId().Should().BeFalse();
    } 
}