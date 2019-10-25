using System.Linq;
using System.Threading.Tasks;
using DFDS.CapabilityService.WebApi.Domain.Exceptions;
using DFDS.CapabilityService.WebApi.Domain.Models;
using DFDS.CapabilityService.WebApi.Domain.Repositories;

namespace DFDS.CapabilityService.WebApi.Domain.Factories
{
    public class CapabilityWithNoDuplicateNameFactory :ICapabilityFactory
    {
        private readonly ICapabilityFactory _inner;
        private readonly ICapabilityRepository _capabilityRepository;

        public CapabilityWithNoDuplicateNameFactory(
            ICapabilityFactory inner, 
            ICapabilityRepository capabilityRepository
        )
        {
            _inner = inner;
            _capabilityRepository = capabilityRepository;
        }

        public async ValueTask<Capability> Create(string name, string description)
        {
            var existingCapabilities = await _capabilityRepository.GetAll();

            var capabilitiesWithSameName = existingCapabilities.Where(c => c.Name == name);
            
            if(capabilitiesWithSameName.Any()) { throw  new CapabilityWithSameNameExistException();}

            return await _inner.Create(name, description);
        }
    }
}