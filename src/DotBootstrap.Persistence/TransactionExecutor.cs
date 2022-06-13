using Microsoft.EntityFrameworkCore;

namespace DotBootstrap.Persistence;

public interface ITransactionExecutor
{
    Task Commit();
}

public class TransactionExecutor : ITransactionExecutor
{
    private readonly DbContext _dbContext;

    public TransactionExecutor(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Commit()
    {
        await _dbContext.SaveChangesAsync();
    }
}