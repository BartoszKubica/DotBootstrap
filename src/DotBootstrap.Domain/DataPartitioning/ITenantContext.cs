namespace DotBootstrap.Domain.DataPartitioning;

public interface ITenantContext
{
    public Guid GetTenantId();
    public bool HasTenantId();
}