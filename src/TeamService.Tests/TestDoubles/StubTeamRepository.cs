using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DFDS.TeamService.WebApi.Models;

namespace DFDS.TeamService.Tests.TestDoubles
{
    public class StubTeamRepository : ITeamRepository
    {
        private readonly Team[] _teams;

        public StubTeamRepository(params Team[] teams)
        {
            _teams = teams;
        }

        public Task<IEnumerable<Team>> GetAll()
        {
            return Task.FromResult(_teams.AsEnumerable());
        }

        public Task Add(Team team)
        {
            return Task.CompletedTask;
        }

        public Task<Team> Get(Guid id)
        {
            var team = _teams.FirstOrDefault();
            return Task.FromResult(team);
        }
    }
}