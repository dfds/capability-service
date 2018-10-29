using System;
using System.Linq;
using System.Threading.Tasks;
using DFDS.TeamService.WebApi.Features.Teams.Application;
using DFDS.TeamService.WebApi.Features.Teams.Domain;
using DFDS.TeamService.WebApi.Features.Teams.Domain.Models;
using DFDS.TeamService.WebApi.Features.Teams.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using static DFDS.TeamService.WebApi.Features.Teams.Infrastructure.Api.DtoHelper;

namespace DFDS.TeamService.WebApi.Features.Teams.Infrastructure.Api
{
    [Route("api/teams")]
    [ApiController]
    public class TeamsController : ControllerBase
    {
        private readonly ITeamApplicationService _teamService;

        public TeamsController(ITeamApplicationService teamService)
        {
            _teamService = teamService;
        }

        [HttpGet("")]
        public async Task<ActionResult<TeamList>> GetAllTeams()
        {
            var teams = await _teamService.GetAllTeams();

            return new TeamList
            {
                Items = teams
                    .Select(ConvertToDto)
                    .ToArray()
            };
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TeamDto>> GetTeam(Guid id)
        {
            var team = await _teamService.GetTeam(id);

            if (team == null)
            {
                return new ActionResult<TeamDto>(NotFound());
            }

            return team.ToDto();
        }

        [HttpPost("")]
        public async Task<IActionResult> CreateTeam([FromBody] CreateTeam createTeam)
        {
            if (string.IsNullOrWhiteSpace(createTeam.Name))
            {
                return BadRequest(new ErrorMessage("Required field \"name\" is missing."));
            }

            var alreadExists = await _teamService.Exists(createTeam.Name);
            if (alreadExists)
            {
                return Conflict(new ErrorMessage($"A team with the given name of \"{createTeam.Name}\" already exists."));
            }

            var team = await _teamService.CreateTeam(createTeam.Name, createTeam.Department);

            return CreatedAtAction(
                actionName: nameof(GetTeam),
                routeValues: new {id = team.Id},
                value: ConvertToDto(team)
            );
        }

        [HttpPost("{id}/members")]
        public async Task<IActionResult> JoinTeam(Guid id, [FromBody] JoinTeam joinTeam)
        {
            if (string.IsNullOrWhiteSpace(joinTeam.UserId))
            { 
                return BadRequest(new ErrorMessage("Required field \"userId\" is missing."));
            }

            try
            {
                await _teamService.JoinTeam(id, joinTeam.UserId);

                var team = await _teamService.GetTeam(id);
                var member = team.FindMemberById(joinTeam.UserId);

                var dto = ConvertToDto(member);

                return Ok(dto);

            }
            catch (AlreadyJoinedException)
            {
                return Conflict(new ErrorMessage($"User with id \"{joinTeam.UserId}\" already member of team with id \"{id}\""));
            }
        }
    }

    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var user = await _userRepository.GetById(id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }
    }
}