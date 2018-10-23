using System;
using System.Threading.Tasks;
using DFDS.TeamService.WebApi.Features.AwsRoles;
using DFDS.TeamService.WebApi.Features.Teams.Application;

namespace DFDS.TeamService.WebApi.Features.AwsConsoleLogin
{
    public class TeamIdToAwsConsoleUrl : ITeamIdToAwsConsoleUrl
    {
        private readonly ITeamApplicationService _teamApplicationService;
        private readonly TeamNameToRoleNameConverter _teamNameToRoleNameConverter;
        private readonly IAwsConsoleUrlBuilder _awsConsoleUrlBuilder;
        private readonly AwsAccountId _awsAccountId;


        public TeamIdToAwsConsoleUrl(
            ITeamApplicationService teamApplicationService, 
            TeamNameToRoleNameConverter teamNameToRoleNameConverter, 
            IAwsConsoleUrlBuilder awsConsoleUrlBuilder,
            AwsAccountId awsAccountId
        )
        {
            _teamApplicationService = teamApplicationService;
            _teamNameToRoleNameConverter = teamNameToRoleNameConverter;
            _awsConsoleUrlBuilder = awsConsoleUrlBuilder;
            _awsAccountId = awsAccountId;
        }

        
        public async Task<Uri> CreateUrlAsync(
            Guid teamId,
            string idToken
        )
        {
            var team = await _teamApplicationService.GetTeam(teamId);

            var roleName = _teamNameToRoleNameConverter.Convert(team.Name);

            var roleArn = CreateRoleArn(
                _awsAccountId, 
                roleName
            );

            var url = await _awsConsoleUrlBuilder.GenerateUriForConsole(
                idToken,
                roleArn
            );


            return url;
        }

        
        public string CreateRoleArn(
            string accountId,
            string roleName
        )
        {
            var roleArn= $"arn:aws:iam::{accountId}:role/{roleName}";


            return roleArn;
        }
    }
}