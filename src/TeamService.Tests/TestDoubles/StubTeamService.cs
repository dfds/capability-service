using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.CognitoIdentityProvider.Model;
using DFDS.TeamService.WebApi.Features.Teams;
using DFDS.TeamService.WebApi.Model;

namespace DFDS.TeamService.Tests.TestDoubles
{
    public class StubTeamService : ITeamService
    {
        private readonly bool _alreadyExists;
        private readonly Team[] _teams;

        public StubTeamService(bool alreadyExists = false, params Team[] teams)
        {
            _alreadyExists = alreadyExists;
            _teams = teams;
        }
        public Task<List<Team>> GetAllTeams()
        {
            return Task.FromResult(new List<Team>(_teams));
        }

        public Task<Team> GetTeam(string id)
        {
            return Task.FromResult(_teams.FirstOrDefault());
        }

        public User CreateUserFromUserAndAttributes(string username, List<AttributeType> attributes)
        {
            throw new System.NotImplementedException();
        }

        public User CreateUserFromUserType(UserType userType)
        {
            throw new System.NotImplementedException();
        }

        public Task<Team> CreateTeam(string name, string department)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> Exists(string teamName)
        {
            return Task.FromResult(_alreadyExists);
        }

        public Task JoinTeam(string teamId, string userId)
        {
            return Task.CompletedTask;
        }
    }
}