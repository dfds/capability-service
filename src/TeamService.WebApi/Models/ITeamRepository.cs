using System.Collections.Generic;
using System.Threading.Tasks;

namespace DFDS.TeamService.WebApi.Models
{
    public interface ITeamRepository
    {
        Task<IEnumerable<Team>> GetAll();
    }
}