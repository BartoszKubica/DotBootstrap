using DotBootstrap.Domain;

namespace DotBootstrap.Persistence;

public interface IDomainMapper<TModel, TEntity> where TModel : class
    where TEntity : Aggregate
{
    public TModel Map(TEntity entity);
    public TEntity Map(TModel model);
}