using System.Linq;
using DFDS.TeamService.WebApi.Features.Teams.Domain.Models;

namespace DFDS.TeamService.WebApi.Features.Teams.Infrastructure.Api
{
    public static class DtoHelper
    {
        public static TeamDto ConvertToDto(Team team)
        {
            return new TeamDto
            {
                Id = team.Id,
                Name = team.Name,
                Department = team.Department,
                Members = team.Members
                    .Select(ConvertToDto)
                    .ToArray()
            };
        }

        public static TeamDto ToDto(this Team team)
        {
            return new TeamDto
            {
                Id = team.Id,
                Name = team.Name,
                Department = team.Department,
                Members = team.Members
                    .Select(ConvertToDto)
                    .ToArray()
            };
        }

        public static UserDto ConvertToDto(User member)
        {
            return new UserDto
            {
                Id = member.Id,
                Name = member.Name,
                Email = member.Email
            };
        }

        public static UserDto ToDto(this User member)
        {
            return new UserDto
            {
                Id = member.Id,
                Name = member.Name,
                Email = member.Email
            };
        }
    }
}