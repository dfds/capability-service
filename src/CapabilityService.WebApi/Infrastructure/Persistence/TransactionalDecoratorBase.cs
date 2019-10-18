using System;
using System.Threading.Tasks;

namespace DFDS.CapabilityService.WebApi.Infrastructure.Persistence
{
    public abstract class TransactionalDecoratorBase
    {
        private readonly CapabilityServiceDbContext _dbContext;

        protected TransactionalDecoratorBase(CapabilityServiceDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        protected async Task ExecuteInTransaction(Func<Task> action)
        {
            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                await action();

                await _dbContext.SaveChangesAsync();
                transaction.Commit();
            }
        }

        protected async Task<T> ExecuteInTransaction<T>(Func<Task<T>> action)
        {
            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                var result = await action();

                await _dbContext.SaveChangesAsync();
                transaction.Commit();

                return result;
            }
        }
    }
}