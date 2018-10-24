using DFDS.TeamService.WebApi.Features.UserServices.model;
using Microsoft.AspNetCore.Mvc;

namespace DFDS.TeamService.WebApi.Features.MyServices
{
    public class UserServicesController
    {
        [HttpGet("api/users/{userId}/services")]
        public TeamsDTO GetServices(string userId)
        {
            var awsConsoleLogin = new ServiceDTO
            {
                Name = "AWS Console",
                Location = "/aws"
            };

            var teamAwesome = new TeamDTO{
                Name = "Awsome",
                Department = "Swimming",
                Services = new []{awsConsoleLogin}
            };

            var teamSecond = new TeamDTO{
                Name = "Second",
                Department = "Swimming",
                Services = new ServiceDTO[0]
            };

            var teamsResponse = new TeamsDTO{
                Teams = new []{
                    teamAwesome, 
                    teamSecond
                }
            };


            return teamsResponse;
        }
    }
}