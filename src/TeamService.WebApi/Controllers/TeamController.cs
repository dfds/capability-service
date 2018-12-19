using Microsoft.AspNetCore.Mvc;

namespace DFDS.TeamService.WebApi.Controllers
{
    [ApiController]
    [Route("api/teams")]
    public class TeamController  : ControllerBase
    {
        [HttpGet("")]
        public IActionResult GetAllTeams()
        {
            return Ok("hello world");
        }
    }
}