using System.Threading.Tasks;

namespace DFDS.TeamService.WebApi.Features.AwsConsoleLogin
{
    public class TeamIdToAwsConsoleUrl : ITeamIdToAwsConsoleUrl
    {
        public Task<string> CreateUrlAsync(string teamId, string idToken)
        {
            return Task.FromResult(string.Empty);
        }
    }
}