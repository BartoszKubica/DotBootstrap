namespace DotBootstrap.Domain.DataPartitioning;

public class TenantContextIsNotSet : BaseException
{
    public TenantContextIsNotSet(string message) : base(message)
    {
    }
}