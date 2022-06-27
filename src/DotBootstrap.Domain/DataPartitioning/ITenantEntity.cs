namespace DotBootstrap.Domain.DataPartitioning;

public interface ITenantEntity
{
    public Guid TenantId { get; }
}