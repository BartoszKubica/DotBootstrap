namespace DotBootstrap.Domain.DataPartitioning;

internal class TenantProvider : ITenantContext, ITenantSetter
{
    private Guid? TenantId { get; set; }

    public void SetTenantId(Guid tenantId)
    {
        TenantId = tenantId;
    }

    public Guid GetTenantId()
    {
        return TenantId ?? throw new TenantContextIsNotSet("Tenant context is not set.");
    }

    public bool HasTenantId()
    {
        return TenantId.HasValue;
    }
}