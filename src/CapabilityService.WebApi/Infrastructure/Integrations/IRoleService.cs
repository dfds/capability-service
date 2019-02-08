using System.Threading.Tasks;
using DFDS.CapabilityService.WebApi.Domain.Models;

namespace DFDS.CapabilityService.WebApi.Infrastructure.Integrations
{
    public interface IRoleService
    {
        Task CreateRoleFor(Capability capability);
    }
}