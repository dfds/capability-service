using System;
using System.Linq;
using System.Threading.Tasks;
using DFDS.TeamService.WebApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace DFDS.TeamService.WebApi.Controllers
{
    [ApiController]
    [Route("api/teams")]
    public class TeamController  : ControllerBase
    {
        private readonly ITeamRepository _teamRepository;

        public TeamController(ITeamRepository teamRepository)
        {
            _teamRepository = teamRepository;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetAllTeams()
        {
            var teams = await _teamRepository.GetAll();

            return Ok(new TeamResponse
            {
                Items = teams
                        .Select(x => new TeamDto
                        {
                            Id = x.Id,
                            Name = x.Name
                        })
                        .ToArray()
            });
        }
    }

    public class TeamResponse
    {
        public TeamDto[] Items { get; set; }
    }

    public class TeamDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}