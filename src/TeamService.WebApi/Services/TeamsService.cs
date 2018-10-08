using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.CognitoIdentityProvider.Model;
using DFDS.TeamService.WebApi.Clients;
using DFDS.TeamService.WebApi.Failures;
using DFDS.TeamService.WebApi.Model;

namespace DFDS.TeamService.WebApi.Services
{
    public class TeamsService
    {
        private readonly UserPoolClient _userPoolClient;

        public TeamsService(UserPoolClient userPoolClient)
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


        public async Task<Result<Team, IFailure>> CreateTeam(CreateTeam createTeam)
        {
            if (string.IsNullOrWhiteSpace(createTeam.Name))
            {
                var validationFailed = new ValidationFailed();
                validationFailed.Add(nameof(createTeam.Name), "can not be empty");
                return new Result<Team, IFailure>(validationFailed);
            }

            var groupName = $"{createTeam.Name}_D_{createTeam.Department}";


            var existingTeam = await _userPoolClient.GetGroupAsync(groupName);
            if (existingTeam != null)
            {
                return new Result<Team, IFailure>(
                    new Conflict($"a team with the name {createTeam.Name} already exists"));
            }

            await _userPoolClient.CreateGroupAsync(groupName);

            var team = new Team
            {
                Id = groupName,
                Name = createTeam.Name,
                Department = createTeam.Department,
                Members = new List<User>()
            };

            return new Result<Team, IFailure>(team);
        }


        public async Task<Result<User, NotFound>> JoinTeam(
            string teamId, 
            string userId
        )
        {
            var addUserToGroupResult = await _userPoolClient.AddUserToGroup(teamId, userId);

            var createUser = await addUserToGroupResult.Reduce(async nothing =>
                {
                    var getAttributesResult = await _userPoolClient.GetUserAttributesAsync(userId);

                    var result = getAttributesResult.Reduce(
                        attributes =>
                        {
                            var user = CreateUserFromUserAndAttributes(
                                userId,
                                attributes
                            );

                            return new Result<User, NotFound>(user);
                        },
                        notFound => new Result<User, NotFound>(notFound));

                    return result;
                },
                notFound => Task.FromResult(new Result<User, NotFound>(notFound))
            );


            return createUser;
        }
    }
}