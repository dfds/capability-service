using System.Threading.Tasks;
using DFDS.TeamService.WebApi.Features.Teams.Domain.Repositories;
using DFDS.TeamService.WebApi.Features.UserServices.model;
using Microsoft.AspNetCore.Mvc;

namespace DFDS.TeamService.WebApi.Features.MyServices
{
    public class UserServicesController: ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UserServicesController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        
        [HttpGet("api/users/{userId}/services")]
        public async Task<ActionResult<TeamsDTO>> GetServices(string userId)
        {
            var user = await _userRepository.GetById(userId);

            if (user == null)
            {
                return new ActionResult<TeamsDTO>(NotFound());
            }
            
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