using DotBootstrap.Domain;

namespace DotBootstrap.Persistence.Exceptions;

public class EntityNotFound : BaseException
{
    private EntityNotFound(string message) : base(message)
    {
    }
    
    public static EntityNotFound Instance<TEntity>(Guid id)
        where TEntity : IEntity
        => new EntityNotFound($"Entity {typeof(TEntity).Name} with id: {id} not found");
}