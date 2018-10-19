using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DFDS.TeamService.WebApi.Features.Teams.Application;
using DFDS.TeamService.WebApi.Features.Teams.Domain.Models;

namespace DFDS.TeamService.Tests.TestDoubles
{
    public class StubTeamService : ITeamApplicationService
    {
        private readonly bool _alreadyExists;
        private readonly Team[] _teams;

        public StubTeamService(bool alreadyExists = false, params Team[] teams)
        {
            _alreadyExists = alreadyExists;
            _teams = teams;
        }
        public Task<IEnumerable<Team>> GetAllTeams()
        {
            return Task.FromResult(_teams.AsEnumerable());
        }

        public Task<Team> GetTeam(Guid id)
        {
            return Task.FromResult(_teams.FirstOrDefault());
        }

        public Task<Team> CreateTeam(string name, string department)
        {
            return Task.FromResult(_teams.FirstOrDefault());
        }

        public Task<bool> Exists(string teamName)
        {
            return Task.FromResult(_alreadyExists);
        }

        public Task JoinTeam(Guid teamId, string userId)
        {
            return Task.CompletedTask;
        }
    }
}