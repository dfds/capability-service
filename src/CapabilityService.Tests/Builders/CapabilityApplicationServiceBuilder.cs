using DFDS.CapabilityService.Tests.TestDoubles;
using DFDS.CapabilityService.WebApi.Application;
using DFDS.CapabilityService.WebApi.Domain.Repositories;
using DFDS.CapabilityService.WebApi.Infrastructure.Integrations;

namespace DFDS.CapabilityService.Tests.Builders
{
    public class CapabilityApplicationServiceBuilder
    {
        private ICapabilityRepository _capabilityRepository;
        private IRoleService _roleService;

        public CapabilityApplicationServiceBuilder()
        {
            _capabilityRepository = Dummy.Of<ICapabilityRepository>();
            _roleService = Dummy.Of<IRoleService>();
        }

        public CapabilityApplicationServiceBuilder WithCapabilityRepository(ICapabilityRepository capabilityRepository)
        {
            _capabilityRepository = capabilityRepository;
            return this;
        }

        public CapabilityApplicationServiceBuilder WithRoleService(IRoleService roleService)
        {
            _roleService = roleService;
            return this;
        }
        
        public CapabilityApplicationService Build()
        {
            return new CapabilityApplicationService(
                capabilityRepository: _capabilityRepository,
                roleService: _roleService
            );
        }
    }
}