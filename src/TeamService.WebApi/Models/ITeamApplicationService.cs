using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DFDS.TeamService.WebApi.Models
{
    public interface ITeamApplicationService
    {
        Task<Team> CreateTeam(string name);
        Task<IEnumerable<Team>> GetAllTeams();
        Task<Team> GetTeam(Guid id);
    }
}