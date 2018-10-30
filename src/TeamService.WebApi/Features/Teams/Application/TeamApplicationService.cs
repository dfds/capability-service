using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DFDS.TeamService.WebApi.Features.Teams.Domain.Models;
using DFDS.TeamService.WebApi.Features.Teams.Domain.Repositories;

namespace DFDS.TeamService.WebApi.Features.Teams.Application
{
    public class TeamApplicationService : ITeamApplicationService
    {
        private readonly ITeamRepository _teamRepository;
        private readonly IUserRepository _userRepository;

        public TeamApplicationService(ITeamRepository teamRepository, IUserRepository userRepository)
        {
            _teamRepository = teamRepository;
            _userRepository = userRepository;
        }

        public Task<IEnumerable<Team>> GetAllTeams()
        {
            return _teamRepository.GetAll();
        }

        public Task<Team> GetTeam(Guid id)
        {
            return _teamRepository.GetById(id);
        }

        public async Task<Team> CreateTeam(string name, string department)
        {
            var team = Team.Create(name, department);
            await _teamRepository.Add(team);

            return team;
        }

        public Task<bool> Exists(string teamName)
        {
            return _teamRepository.ExistsWithName(teamName);
        }

        public async Task JoinTeam(Guid teamId, string userId)
        {
            var team = await _teamRepository.GetById(teamId);
            if (team == null)
            {
                throw new Exception($"Team with id \"{teamId}\" could not be found. User \"{userId}\" can therefore not join that team.");
            }

            var user = await _userRepository.GetById(userId);
            if (user == null)
            {
                throw new Exception($"User with id\"{userId}\" could not be found and can therefore not join team \"{team.Name}\".");
            }

            team.StartMembership(user, MembershipType.Developer);
        }
    }
}