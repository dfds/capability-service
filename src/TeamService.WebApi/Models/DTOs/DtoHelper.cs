namespace DFDS.TeamService.WebApi.Models.DTOs
{
    public static class DtoHelper
    {
        public static Team ConvertToDto(Models.Team team)
        {
            return new Team
            {
                Id = team.Id,
                Name = team.Name,
                Members = new Member[0]
            };
        }
    }
}