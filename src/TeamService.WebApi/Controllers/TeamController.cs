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
                        .Select(ConvertToDto)
                        .ToArray()
            });
        }

        [HttpPost("")]
        public async Task<IActionResult> CreateTeam(TeamInput input)
        {
            var team = Team.Create(input.Name);
            await _teamRepository.Add(team);

            var dto = ConvertToDto(team);

            return CreatedAtAction(
                actionName: nameof(GetAllTeams),
                routeValues: new {id = team.Id},
                value: dto
            );
        }
    }
}