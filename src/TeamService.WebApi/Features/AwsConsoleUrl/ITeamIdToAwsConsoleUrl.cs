using System;
using System.Threading.Tasks;

namespace DFDS.TeamService.WebApi.Features.AwsConsoleLogin
{
    public interface ITeamIdToAwsConsoleUrl
    {
        Task<Uri> CreateUrlAsync(
            Guid teamId,
            string idToken
        );
    }
}