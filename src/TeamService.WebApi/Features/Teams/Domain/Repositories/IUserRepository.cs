using System.Threading.Tasks;
using DFDS.TeamService.WebApi.Features.Teams.Domain.Models;

namespace DFDS.TeamService.WebApi.Features.Teams.Domain.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetById(string userId);
    }
}