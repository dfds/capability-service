using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DFDS.TeamService.WebApi.Models
{
    public interface ITeamRepository
    {
        Task<IEnumerable<Team>> GetAll();
        Task Add(Team team);
        Task<Team> Get(Guid id);
    }
}