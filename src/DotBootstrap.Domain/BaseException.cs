namespace DotBootstrap.Domain;

public class BaseException : Exception
{
    public BaseException(string message) : base(message) { }
}