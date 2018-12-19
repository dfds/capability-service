using System.Collections.Generic;
using System.Threading.Tasks;
using DFDS.TeamService.WebApi.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DFDS.TeamService.WebApi.Models
{
    public class TeamRepository : ITeamRepository
    {
        private readonly TeamServiceDbContext _dbContext;

        public TeamRepository(TeamServiceDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Team>> GetAll()
        {
            return await _dbContext.Teams.ToListAsync();
        }

        public async Task Add(Team team)
        {
            await _dbContext.Teams.AddAsync(team);
            await _dbContext.SaveChangesAsync();
        }
    }
}