using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.CognitoIdentityProvider.Model;
using DFDS.TeamService.WebApi.Model;

namespace DFDS.TeamService.WebApi.Features.Teams
{
    public interface ITeamService
    {
        Task<List<Team>> GetAllTeams();
        Task<Team> GetTeam(string id);

        User CreateUserFromUserAndAttributes(
            string username,
            List<AttributeType> attributes
        );

        User CreateUserFromUserType(UserType userType);

        Task<Team> CreateTeam(string name, string department);
        Task<bool> Exists(string teamName);
        Task JoinTeam(string teamId, string userId);
    }
}