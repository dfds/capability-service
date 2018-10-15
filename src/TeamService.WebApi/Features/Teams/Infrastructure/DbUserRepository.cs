using System.Threading.Tasks;
using DFDS.TeamService.WebApi.Features.Teams.Domain.Models;
using DFDS.TeamService.WebApi.Features.Teams.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DFDS.TeamService.WebApi.Features.Teams.Infrastructure
{
    public class DbUserRepository : IUserRepository
    {
        private readonly TeamServiceDbContext _dbContext;

        public DbUserRepository(TeamServiceDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<User> GetById(string userId)
        {
            return await _dbContext.Users.SingleOrDefaultAsync(x => x.Id == userId);
        }
    }
}