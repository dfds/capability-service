using System.Threading.Tasks;

namespace DFDS.CapabilityService.WebApi.Models
{
    public interface IRoleService
    {
        Task CreateRoleFor(Capability capability);
    }
}