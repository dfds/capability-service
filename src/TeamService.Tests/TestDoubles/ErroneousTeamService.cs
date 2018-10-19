using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DFDS.TeamService.WebApi.Features.Teams.Application;
using DFDS.TeamService.WebApi.Features.Teams.Domain.Models;

namespace DFDS.TeamService.Tests.TestDoubles
{
    public class ErroneousTeamService : ITeamApplicationService
    {
        private readonly Exception _exception;

        public ErroneousTeamService(Exception exception)
        {
            _exception = exception;
        }

        public Task<IEnumerable<Team>> GetAllTeams()
        {
            throw new NotImplementedException();
        }

        public Task<Team> GetTeam(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<Team> CreateTeam(string name, string department)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Exists(string teamName)
        {
            throw new NotImplementedException();
        }

        public Task JoinTeam(Guid teamId, string userId)
        {
            throw _exception;
        }
    }
}