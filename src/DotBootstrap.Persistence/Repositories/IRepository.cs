namespace DotBootstrap.Persistence.Repositories;

public interface IRepository<TEntity> where TEntity : class
{
    Task Add(TEntity entity, CancellationToken cancellationToken);
    Task Delete(TEntity entity, long version);
    Task Update(TEntity entity, long version);
    Task<TEntity> Get(Guid id);
    Task<TEntity?> Search(Guid id);
}