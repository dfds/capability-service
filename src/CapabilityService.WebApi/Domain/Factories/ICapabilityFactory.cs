using DFDS.CapabilityService.WebApi.Domain.Models;

namespace DFDS.CapabilityService.WebApi.Domain.Factories
{
    public interface ICapabilityFactory
    {
        Capability Create(string name, string description);
    }
}