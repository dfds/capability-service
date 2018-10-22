using System;
using System.Threading.Tasks;
using DFDS.TeamService.WebApi.Features.AwsRoles;
using Microsoft.AspNetCore.Mvc;

namespace DFDS.TeamService.WebApi.Features.AwsConsoleLogin
{
    [ApiController]
    public class AwsConsoleUrlController : ControllerBase
    {
        private readonly IAwsConsoleUrlBuilder _awsConsoleUrlBuilder;
        private readonly TeamNameToRoleNameConverter _teamNameToRoleNameConverter;
        
        public AwsConsoleUrlController(
            IAwsConsoleUrlBuilder awsConsoleUrlBuilder, 
            TeamNameToRoleNameConverter teamNameToRoleNameConverter
        )
        {
            _awsConsoleUrlBuilder = awsConsoleUrlBuilder;
            _teamNameToRoleNameConverter = teamNameToRoleNameConverter;
        }

        
        [HttpGet("aws/console-url")]
        public async Task<ActionResult<AWSConsoleLinkResponse>> GetConsoleUrl(
            [FromQuery] string idToken,
            [FromQuery] string teamName
        )
        {
            var roleName = _teamNameToRoleNameConverter.Convert(teamName);
            var consoleLink = await _awsConsoleUrlBuilder.GenerateUriForConsole(
                idToken,
                roleName
            );
            
            
            return new AWSConsoleLinkResponse(consoleLink.AbsoluteUri);
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