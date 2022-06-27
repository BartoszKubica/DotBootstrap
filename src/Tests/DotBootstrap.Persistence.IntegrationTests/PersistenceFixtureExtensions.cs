using System;
using System.Threading.Tasks;

namespace DotBootstrap.Persistence.IntegrationTests;

public static class PersistenceFixtureExtensions
{
    public static Task InvokeInTenantScope(this PersistenceFixture fixture, Func<Task> action, Guid? tenantId = null)
    {
        fixture.TenantSetter.SetTenantId(tenantId ?? Guid.NewGuid());

        return action();
    }
}