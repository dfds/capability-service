using System;
using System.Threading.Tasks;
using DFDS.TeamService.WebApi.Features.Teams.Application;

namespace DFDS.TeamService.WebApi.Features.AwsConsoleLogin
{
    public class TeamIdToAwsConsoleUrl : ITeamIdToAwsConsoleUrl
    {
        private readonly ITeamApplicationService _teamApplicationService;

        public TeamIdToAwsConsoleUrl(ITeamApplicationService teamApplicationService)
        {
            _teamApplicationService = teamApplicationService;
        }

        public Task<Uri> CreateUrlAsync(
            Guid teamId,
            string idToken
        )
        {
            throw new NotImplementedException();
        }

    }
}