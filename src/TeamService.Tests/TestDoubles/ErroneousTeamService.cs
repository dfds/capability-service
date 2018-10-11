using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.CognitoIdentityProvider.Model;
using DFDS.TeamService.WebApi.Features.Teams;

namespace DFDS.TeamService.Tests.TestDoubles
{
    public class ErroneousTeamService : ITeamService
    {
        private readonly Exception _exception;

        public ErroneousTeamService(Exception exception)
        {
            _exception = exception;
        }

        public Task<List<Team>> GetAllTeams()
        {
            throw new NotImplementedException();
        }

        public Task<Team> GetTeam(string id)
        {
            throw new NotImplementedException();
        }

        public User CreateUserFromUserAndAttributes(string username, List<AttributeType> attributes)
        {
            throw new NotImplementedException();
        }

        public User CreateUserFromUserType(UserType userType)
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

        public Task JoinTeam(string teamId, string userId)
        {
            throw _exception;
        }
    }
}