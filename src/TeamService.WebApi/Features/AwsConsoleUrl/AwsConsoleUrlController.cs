using System;
using System.Threading.Tasks;
using DFDS.TeamService.WebApi.Features.AwsRoles;
using Microsoft.AspNetCore.Mvc;

namespace DFDS.TeamService.WebApi.Features.AwsConsoleLogin
{
    [ApiController]
    public class AwsConsoleUrlController : ControllerBase
    {
        private readonly ITeamIdToAwsConsoleUrl _teamIdToAwsConsoleUrl;
        
        public AwsConsoleUrlController(
            ITeamIdToAwsConsoleUrl teamIdToAwsConsoleUrl
        )
        {
            _teamIdToAwsConsoleUrl = teamIdToAwsConsoleUrl;
        }

        
        [HttpGet("api/teams/{id}/aws/console-url")]
        public async Task<ActionResult<AWSConsoleLinkResponse>> GetConsoleUrl(
            string id,
            [FromQuery] string idToken
        )
        {

            var url = await _teamIdToAwsConsoleUrl.CreateUrlAsync(
                id,
                idToken
            );
            
            
            return new AWSConsoleLinkResponse(url);
        }
    }

    
    public interface IAwsConsoleUrlBuilder
    {
        Task<Uri> GenerateUriForConsole(
            string idToken, 
            string roleName
        );
    }


    public class AWSConsoleLinkResponse
    {

        public AWSConsoleLinkResponse(string absoluteUrl)
        {
            AbsoluteUrl = absoluteUrl;
        }
        public string AbsoluteUrl { get; }
    }




}