using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DFDS.TeamService.WebApi.Models
{
    public class TeamApplicationService : ITeamApplicationService
    {
        private readonly ITeamRepository _teamRepository;

        public TeamApplicationService(ITeamRepository teamRepository)
        {
            _teamRepository = teamRepository;
        }

        public async Task<Team> CreateTeam(string name)
        {
            var team = Team.Create(name);
            await _teamRepository.Add(team);

            return team;
        }

        public Task<Team> GetTeam(Guid id) => _teamRepository.Get(id);
        public Task<IEnumerable<Team>> GetAllTeams() => _teamRepository.GetAll();
    }
}