using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace DFDS.TeamService.WebApi.Persistence
{
    public class UnitOfWork<TDbContext> : IUnitOfWork<TDbContext> where TDbContext : DbContext
    {
        public UnitOfWork(TDbContext dbContext)
        {
            DbContext = dbContext;
        }

        protected TDbContext DbContext { get; }

        protected virtual Task OnPreTransactionCommit(IDbContextTransaction transaction)
        {
            return Task.CompletedTask;
        }

        protected virtual Task OnPostTransactionCommitted(IDbContextTransaction transaction)
        {
            return Task.CompletedTask;
        }

        protected virtual async Task ExecuteInTransactionScope(Func<Task> action)
        {
            using (var transaction = await DbContext.Database.BeginTransactionAsync())
            {
                await action();

                await OnPreTransactionCommit(transaction);

                await DbContext.SaveChangesAsync();
                transaction.Commit();

                await OnPostTransactionCommitted(transaction);
            }
        }

        public Task Run(Func<Task> action)
        {
            return ExecuteInTransactionScope(action);
        }

        public async Task<TResult> Run<TResult>(Func<Task<TResult>> action)
        {
            var result = default(TResult);

            await ExecuteInTransactionScope(async () =>
            {
                result = await action();
            });

            return result;
        }
    }
}