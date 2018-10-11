using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.CognitoIdentityProvider.Model;
using DFDS.TeamService.WebApi.Clients;

namespace DFDS.TeamService.WebApi.Features.Teams
{
    public class TeamService : ITeamService
    {
        private readonly UserPoolClient _userPoolClient;

        public TeamService(UserPoolClient userPoolClient)
        {
            _userPoolClient = userPoolClient;
        }

        public async Task<List<Team>> GetAllTeams()
        {
            var groupNames = await _userPoolClient.ListGroupsAsync();

            var getTeamsTask = groupNames
                .Select(g => GetTeam(g));


            var teams = (await Task.WhenAll(getTeamsTask)).ToList();


            return teams;
        }

        public async Task<Team> GetTeam(string id)
        {
            var usersInGroup = await _userPoolClient.ListUsersInGroupAsync(id);

            var teamNameAndDepartment = id.Split(new[] {"_D_"}, StringSplitOptions.None);
            var departmentName = 1 < teamNameAndDepartment.Length ? teamNameAndDepartment[1] : null;

            var team = new Team
            {
                Id = id,
                Name = teamNameAndDepartment[0],
                Department = departmentName,
                Members = usersInGroup
                    .Select(u => CreateUserFromUserType(u)
                    ).ToList()
            };


            return team;
        }


        public User CreateUserFromUserAndAttributes(
            string username,
            List<AttributeType> attributes
        )
        {
            var user = new User();
            user.Id = username;

            if (attributes == null)
            {
                return user;
            }

            user.Name = attributes.FirstOrDefault(a => a.Name == "name")?.Value;
            user.Email = attributes.FirstOrDefault(a => a.Name == "email")?.Value;


            return user;
        }


        public User CreateUserFromUserType(UserType userType)
        {
            var user = CreateUserFromUserAndAttributes(
                userType.Username,
                userType.Attributes
            );


            return user;
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
            throw new NotImplementedException();
        }
    }
}