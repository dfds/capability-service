using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DFDS.TeamService.WebApi.Features.Teams.Domain.Models;
using DFDS.TeamService.WebApi.Features.Teams.Domain.Repositories;
using DFDS.TeamService.WebApi.Features.UserServices.model;
using Microsoft.AspNetCore.Mvc;

namespace DFDS.TeamService.WebApi.Features.MyServices
{
    [ApiController]
    public class UserServicesController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ITeamRepository _teamRepository;
        
        public UserServicesController(
            IUserRepository userRepository, 
            ITeamRepository teamRepository
        )
        {
            _userRepository = userRepository;
            _teamRepository = teamRepository;
        }
        
        
        [HttpGet("api/users/{userId}/services")]
        public async Task<ActionResult<TeamsDTO>> GetServices(string userId)
        {
            var user = await _userRepository.GetById(userId);

            if (user == null)
            {
                return new ActionResult<TeamsDTO>(NotFound($"User with id: {userId} not found"));
            }

            
            var teamsWithUser = await _teamRepository.GetByUserId(userId);
     
            
            var teamsResponse = new TeamsDTO
            {
                Items = teamsWithUser.Select(t =>
                    CreateTeam(t)
                )
            };


            return teamsResponse;
        }

        private TeamDTO CreateTeam(Team team)
        {
            var teamDTO = new TeamDTO
            {
                Name = team.Name,
                Department = team.Department,
                Services = CreateServices(team)
            };

            return teamDTO;
        }

        private IEnumerable<ServiceDTO> CreateServices(Team team)
        {
            var services = new List<ServiceDTO>();
            services.Add(CreateAwsConsoleService(team.Id));

            return services;
        }

        private ServiceDTO CreateAwsConsoleService(Guid teamId)
        {
            return new ServiceDTO
            {
                Name = "AWS Console",
                Location = $"/api/teams/{teamId}/aws/console-url"
            };
        }
    }
}