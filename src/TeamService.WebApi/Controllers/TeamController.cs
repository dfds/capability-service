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
    [Route("api/v1/teams")]
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
            try {
                var team = await _teamApplicationService.CreateTeam(input.Name);
                var dto = ConvertToDto(team);

                return CreatedAtAction(
                    actionName: nameof(GetTeam),
                    routeValues: new {id = team.Id},
                    value: dto
                );
            } catch (TeamValidationException tve) {
                return BadRequest(tve.Message);
            }
        }

        [HttpPost("{id}/members")]
        public async Task<IActionResult> AddMemberToTeam(string id, [FromBody] MemberInput input)
        {
            var teamId = Guid.Empty;
            Guid.TryParse(id, out teamId);

            try
            {
                await _teamApplicationService.JoinTeam(teamId, input.Email);
            }
            catch (TeamDoesNotExistException)
            {
                return NotFound(new
                {
                    Message = $"Team with id {id} could not be found."
                });
            }

            return Ok();
        }

        [HttpDelete("{id}/members/{memberEmail}")]
        public async Task<IActionResult> RemoveMemberFromTeam([FromRoute] string id, [FromRoute] string memberEmail)
        {
            var teamId = Guid.Empty;
            Guid.TryParse(id, out teamId);

            try
            {
                await _teamApplicationService.LeaveTeam(teamId, memberEmail);
            }
            catch (TeamDoesNotExistException)
            {
                return NotFound(new
                {
                    Message = $"Team with id {id} could not be found."
                });
            }
            catch (NotMemberOfTeamException)
            {
                return NotFound(new
                {
                    Message = $"Team with id {id} does not have member \"{memberEmail}\"."
                });
            }

            return Ok();
        }
    }

    public class MemberInput
    {
        public string Email { get; set; }
    }
}