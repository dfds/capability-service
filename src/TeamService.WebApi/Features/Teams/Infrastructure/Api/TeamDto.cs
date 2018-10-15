using System;

namespace DFDS.TeamService.WebApi.Features.Teams.Infrastructure.Api
{
    public class TeamDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Department { get; set; }
        public UserDto[] Members { get; set; }
    }
}