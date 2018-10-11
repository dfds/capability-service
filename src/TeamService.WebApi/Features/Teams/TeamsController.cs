using System.Linq;
using System.Threading.Tasks;
using DFDS.TeamService.WebApi.Model;
using Microsoft.AspNetCore.Mvc;

namespace DFDS.TeamService.WebApi.Features.Teams
{
    [Route("api/teams")]
    [ApiController]
    public class TeamsController : ControllerBase
    {
        private readonly ITeamService _teamService;

        public TeamsController(ITeamService teamService)
        {
            _teamService = teamService;
        }

        [HttpGet]
        public async Task<ActionResult<TeamList>> GetAllTeams()
        {
            var teams = await _teamService.GetAllTeams();

            return new TeamList
            {
                Items = teams
            };
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Team>> GetTeam(string id)
        {
            var team = await _teamService.GetTeam(id);

            if (team == null)
            {
                return new ActionResult<Team>(NotFound());
            }

            return team;
        }

        [HttpPost("")]
        public async Task<IActionResult> CreateTeam([FromBody] CreateTeam createTeam)
        {
            if (string.IsNullOrWhiteSpace(createTeam.Name))
            {
                return BadRequest(new
                {
                    Message = "Required field \"name\" is missing."
                });
            }

            var alreadExists = await _teamService.Exists(createTeam.Name);
            if (alreadExists)
            {
                return Conflict(new
                {
                    Message = $"A team with the given name of \"{createTeam.Name}\" already exists."
                });
            }

            return CreatedAtAction(nameof(GetTeam), new {id = "1"}, "2");
        }

        [HttpPost("{id}/members")]
        public async Task<IActionResult> JoinTeam(string id, [FromBody] JoinTeam joinTeam)
        {
            if (string.IsNullOrWhiteSpace(joinTeam.UserId))
            {
                return BadRequest(new
                {
                    Message = "Required field \"userId\" is missing."
                });
            }

            try
            {
                await _teamService.JoinTeam(id, joinTeam.UserId);

                var team = await _teamService.GetTeam(id);
                var member = team.Members.SingleOrDefault(x => x.Id == joinTeam.UserId);

                return Ok(member);

            }
            catch (AlreadyJoinedException)
            {
                return Conflict(new
                {
                    Message = $"User with id \"{joinTeam.UserId}\" already member of team with id \"{id}\""
                });
            }
        }
    }
}