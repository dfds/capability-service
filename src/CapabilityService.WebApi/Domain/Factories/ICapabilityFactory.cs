using System.Threading.Tasks;
using DFDS.CapabilityService.WebApi.Domain.Models;

namespace DFDS.CapabilityService.WebApi.Domain.Factories
{
    public interface ICapabilityFactory
    {
        ValueTask<Capability> Create(string name, string description);
    }
}