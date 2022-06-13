using DotBootstrap.Domain;
using DotBootstrap.Messaging.Events;
using DotBootstrap.Persistence.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace DotBootstrap.Persistence.Repositories;

internal class AggregateRepository<TModel, TEntity> : IRepository<TEntity> where TEntity : Aggregate
where TModel : class, IVersionedEntity
{
    private readonly IEventBus _eventBus;
    private readonly DbSet<TModel> _dbSet;
    private readonly DbContext _dbContext;
    private readonly Func<IQueryable<TModel>, IQueryable<TModel>>? _queryTransformation;
    private readonly IDomainMapper<TModel, TEntity> _mapper;
    private IQueryable<TModel> QueryBase => _queryTransformation is null ? _dbSet : _queryTransformation(_dbSet); 

    public AggregateRepository(IEventBus eventBus, DbContext dbContext, IDomainMapper<TModel, TEntity> mapper,
        Func<IQueryable<TModel>, IQueryable<TModel>>? queryTransformation = null)
    {
        _eventBus = eventBus;
        _dbContext = dbContext;
        _queryTransformation = queryTransformation;
        _mapper = mapper;
        _dbSet = _dbContext.Set<TModel>();
    }

    public async Task Add(TEntity entity, CancellationToken cancellationToken)
    {
        await _dbSet.AddAsync(_mapper.Map(entity), cancellationToken);
        await PublishEvents(entity);
    }

    public async Task Delete(TEntity entity, long version)
    {
        var dbEntity = await SearchDb(entity.Id) ?? throw EntityNotFound.Instance<TEntity>(entity.Id);
        var initialVersion = GetVersion(dbEntity);
        if(!initialVersion.Equals(version))
            throw OptimisticConcurrencyException.Instance;
        
        _dbSet.Remove(dbEntity);
        await PublishEvents(entity);
    }

    public async Task Update(TEntity entity, long version)
    {
        var dbEntity = await SearchDb(entity.Id) ?? throw EntityNotFound.Instance<TEntity>(entity.Id);

        var initialVersion = GetVersion(dbEntity);
        if(!initialVersion.Equals(version))
            throw OptimisticConcurrencyException.Instance;
        
        _dbContext.Entry(dbEntity).CurrentValues.SetValues(_mapper.Map(entity));
        await PublishEvents(entity);
    }

    public async Task<TEntity> Get(Guid id)
    {
        return await Search(id) ?? throw EntityNotFound.Instance<TEntity>(id);
    }

    public async Task<TEntity?> Search(Guid id)
    {
        var entity = await SearchDb(id);

        return entity is null ? null : _mapper.Map(entity);
    }

    private async Task PublishEvents(TEntity entity)
    {
        foreach (var @event in entity.DequeueAllEvents())
        {
            await _eventBus.Publish(@event);
        }
    }

    private async Task<TModel?> SearchDb(Guid id)
    {
        return _dbSet.Local.SingleOrDefault(x => x.Id == id)
               ?? await QueryBase.SingleOrDefaultAsync(x => x.Id == id);
    }
    private long GetVersion(TModel entity) => _dbContext.Entry(entity).Property(x => x.Version).OriginalValue;
}