using System;
using System.Threading.Tasks;

namespace DFDS.TeamService.WebApi.Features.AwsConsoleLogin
{
    public class TeamIdToAwsConsoleUrl : ITeamIdToAwsConsoleUrl
    {
        public Task<Uri> CreateUrlAsync(
            string teamId, 
            string idToken
        )
        {
            throw new NotImplementedException();
        }
    }
}