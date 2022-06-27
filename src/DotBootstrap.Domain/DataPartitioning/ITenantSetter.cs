namespace DotBootstrap.Domain.DataPartitioning;

public interface ITenantSetter
{
    void SetTenantId(Guid tenantId);
}