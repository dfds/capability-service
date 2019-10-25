using DFDS.CapabilityService.Tests.TestDoubles;
using DFDS.CapabilityService.WebApi.Application;
using DFDS.CapabilityService.WebApi.Domain.Factories;
using DFDS.CapabilityService.WebApi.Domain.Repositories;

namespace DFDS.CapabilityService.Tests.Builders
{
    public class CapabilityApplicationServiceBuilder
    {
        private ICapabilityRepository _capabilityRepository;
        private ITopicRepository _topicRepository;
        private ICapabilityFactory _capabilityFactory;
        public CapabilityApplicationServiceBuilder()
        {
            _capabilityRepository = Dummy.Of<ICapabilityRepository>();
            _topicRepository = Dummy.Of<ITopicRepository>();
            _capabilityFactory = Dummy.Of<ICapabilityFactory>();
        }

        public CapabilityApplicationServiceBuilder WithCapabilityRepository(ICapabilityRepository capabilityRepository)
        {
            _capabilityRepository = capabilityRepository;
            return this;
        }

        public CapabilityApplicationServiceBuilder WithTopicRepository(ITopicRepository topicRepository)
        {
            _topicRepository = topicRepository;
            return this;
        }
        
        public CapabilityApplicationServiceBuilder WithCapabilityFactory(ICapabilityFactory capabilityFactory)
        {
            _capabilityFactory = capabilityFactory;
            return this;
        }
        
        public CapabilityApplicationService Build()
        {
            return new CapabilityApplicationService(
                capabilityRepository: _capabilityRepository,
                topicRepository: _topicRepository,
                capabilityFactory: _capabilityFactory
            );
        }
    }
}