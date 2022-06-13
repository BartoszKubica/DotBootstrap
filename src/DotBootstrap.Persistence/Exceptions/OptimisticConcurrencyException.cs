using DotBootstrap.Domain;

namespace DotBootstrap.Persistence.Exceptions;

public class OptimisticConcurrencyException : BaseException
{
    public static OptimisticConcurrencyException Instance = new();
    private OptimisticConcurrencyException() : base("Entity has been modified by someone else.")
    {
    }
}