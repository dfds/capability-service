using System;
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
            return await _dbContext
                         .Teams
                         .Include(x => x.Members)
                         .ToListAsync();
        }

        public async Task Add(Team team)
        {
            await _dbContext.Teams.AddAsync(team);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Team> Get(Guid id)
        {
            var team = await _dbContext
                             .Teams
                             .Include(x => x.Members)
                             .SingleOrDefaultAsync(x => x.Id == id);
            return team;
        }
    }
}