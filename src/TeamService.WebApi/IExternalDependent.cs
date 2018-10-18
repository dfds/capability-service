using System.Threading.Tasks;

namespace DFDS.TeamService.WebApi
{
    public interface IExternalDependent
    {
        Task<Status> GetStatusAsync();
    }
}