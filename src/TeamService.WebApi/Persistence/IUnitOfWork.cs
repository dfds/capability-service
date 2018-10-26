using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DFDS.TeamService.WebApi.Persistence
{
    public interface IUnitOfWork<TDbContext> where TDbContext : DbContext
    {
        Task Run(Func<Task> action);
        Task<TResult> Run<TResult>(Func<Task<TResult>> action);
    }
}