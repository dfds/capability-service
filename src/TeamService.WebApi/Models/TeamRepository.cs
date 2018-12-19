using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DFDS.TeamService.WebApi.Models
{
    public class TeamRepository : ITeamRepository
    {
        public Task<IEnumerable<Team>> GetAll()
        {
            var teams = new[]
            {
                new Team(
                    id: Guid.NewGuid(),
                    name: "foo"
                ),
            };

            return Task.FromResult<IEnumerable<Team>>(teams);
        }
    }
}