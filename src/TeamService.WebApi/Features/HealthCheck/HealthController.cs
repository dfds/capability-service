using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace DFDS.TeamService.WebApi.Controllers
{
    [Route("system")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        public HealthController()
        {
        }

        [HttpGet("health")]
        [ProducesResponseType(200, Type = typeof(string))]
        [ProducesResponseType(504, Type = typeof(string))]
        public async Task<ActionResult<string>> Get(bool deep)
        {
            const string allISWell="Cognito WebApi says this is fine";
            
            return allISWell;
//            if (deep == false)
//            {
//               
//            }
//
//            var cognitoIsAlive = await _cognitoClient.IsAlive();
//
//            return cognitoIsAlive ? Ok(allISWell): StatusCode(504, "No connection to AWS cognito can be made");
        }
    }
}