using DotBootstrap.Domain;
using DotBootstrap.Domain.DataPartitioning;
using DotBootstrap.Persistence.Exceptions;

namespace DotBootstrap.Persistence.Repositories;

public class TenantRepositoryDecorator <TEntity> : IRepository<TEntity> where TEntity : Aggregate
{
    private readonly IRepository<TEntity> _decorated;
    private readonly ITenantContext _tenantContext;

    public TenantRepositoryDecorator(IRepository<TEntity> decorated, ITenantContext tenantContext)
    {
        _decorated = decorated;
        _tenantContext = tenantContext;
    }

    public async Task Add(TEntity entity, CancellationToken cancellationToken)
    {
        CheckTenancy(entity);

        await _decorated.Add(entity, cancellationToken);
    }
    

    public async Task Delete(TEntity entity, long version)
    {
        CheckTenancy(entity);


        await _decorated.Delete(entity, version);
    }

    public async Task Update(TEntity entity, long version)
    {
        CheckTenancy(entity);

        await _decorated.Update(entity, version);
    }

    public async Task<TEntity> Get(Guid id)
    {
        var entity = await _decorated.Get(id);
        CheckTenancy(entity);

        return entity;
    }

    public async Task<TEntity?> Search(Guid id)
    {
        var entity = await _decorated.Search(id);
        
        if(entity != null)
            CheckTenancy(entity);

        return entity;
    }

    private void CheckTenancy(TEntity entity)
    {
        var e = (ITenantEntity)entity;
        if (e.TenantId != _tenantContext.GetTenantId())
            throw OutOfTenantException.Instance;
    }
}