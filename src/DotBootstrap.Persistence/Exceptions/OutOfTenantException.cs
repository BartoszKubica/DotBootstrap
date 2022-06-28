using DotBootstrap.Domain;

namespace DotBootstrap.Persistence.Exceptions;

public class OutOfTenantException : BaseException
{
    private OutOfTenantException(string message) : base(message)
    {
    }

    public static OutOfTenantException Instance
        => new OutOfTenantException("Attempt to access data out of the tenant.");
}