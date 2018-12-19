using System;
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
            return Ok(new TeamResponse
            {
                Items = new []
                {
                    new Team
                    {
                        Id = Guid.NewGuid(),
                        Name = "foo"
                    }, 
                }
            });
        }
    }

    public class TeamResponse
    {
        public Team[] Items { get; set; }
    }

    public class Team
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}