using System.Threading.Tasks;

namespace DFDS.TeamService.WebApi.Features.AwsConsoleLogin
{
    public interface ITeamIdToAwsConsoleUrl
    {
        Task<string> CreateUrlAsync(
            string teamId,
            string idToken
        );
    }
}