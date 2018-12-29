using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DFDS.TeamService.WebApi.Controllers;
using DFDS.TeamService.WebApi.Models;

namespace DFDS.TeamService.Tests.TestDoubles
{
    public class StubTeamApplicationService : ITeamApplicationService
    {
        private readonly Team[] _stubTeams;

        public StubTeamApplicationService(params Team[] stubTeams)
        {
            _stubTeams = stubTeams;
        }

        public Task<Team> CreateTeam(string name)
        {
            var team = _stubTeams.FirstOrDefault();
            return Task.FromResult(team);
        }

        public Task<IEnumerable<Team>> GetAllTeams()
        {
            return Task.FromResult(_stubTeams.AsEnumerable());
        }

        public Task<Team> GetTeam(Guid id)
        {
            var team = _stubTeams.FirstOrDefault();
            return Task.FromResult(team);
        }

        public Task JoinTeam(Guid teamId, string memberEmail)
        {
            return Task.CompletedTask;
        }
    }
}