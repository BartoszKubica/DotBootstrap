namespace DotBootstrap.Domain;

public interface IVersionedEntity : IEntity
{
    public long Version { get; }
}