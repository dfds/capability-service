using System;
using System.Threading.Tasks;

namespace DFDS.TeamService.WebApi.Features.AwsConsoleLogin
{
    public interface ITeamIdToAwsConsoleUrl
    {
        Task<Uri> CreateUrlAsync(
            string teamId,
            string idToken
        );
    }
}