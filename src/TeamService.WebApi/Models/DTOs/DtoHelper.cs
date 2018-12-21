namespace DFDS.TeamService.WebApi.Models.DTOs
{
    public static class DtoHelper
    {
        public static TeamDto ConvertToDto(Team team)
        {
            return new TeamDto
            {
                Id = team.Id,
                Name = team.Name
            };
        }
    }
}