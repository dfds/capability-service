using System;
using System.Linq;
using System.Threading.Tasks;
using DFDS.TeamService.WebApi.Models;
using DFDS.TeamService.WebApi.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using static DFDS.TeamService.WebApi.Models.DTOs.DtoHelper;

namespace DFDS.TeamService.WebApi.Controllers
{
    [ApiController]
    [Route("api/teams")]
    public class TeamController : ControllerBase
    {
        private readonly ITeamApplicationService _teamApplicationService;

        public TeamController(ITeamApplicationService teamApplicationService)
        {
            _teamApplicationService = teamApplicationService;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetAllTeams()
        {
            var teams = await _teamApplicationService.GetAllTeams();

            return Ok(new TeamResponse
            {
                Items = teams
                        .Select(ConvertToDto)
                        .ToArray()
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTeam(string id)
        {
            var teamId = Guid.Empty;
            Guid.TryParse(id, out teamId);

            var team = await _teamApplicationService.GetTeam(teamId);

            if (team == null)
            {
                return NotFound(new
                {
                    Message = $"A team with id \"{id}\" could not be found."
                });
            }

            var dto = ConvertToDto(team);

            return Ok(dto);
        }

        [HttpPost("")]
        public async Task<IActionResult> CreateTeam(TeamInput input)
        {
            var team = await _teamApplicationService.CreateTeam(input.Name);
            var dto = ConvertToDto(team);

            return CreatedAtAction(
                actionName: nameof(GetTeam),
                routeValues: new {id = team.Id},
                value: dto
            );
        }
    }
}