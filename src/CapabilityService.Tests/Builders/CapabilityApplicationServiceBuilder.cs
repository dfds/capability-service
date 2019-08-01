using DFDS.CapabilityService.Tests.TestDoubles;
using DFDS.CapabilityService.WebApi.Application;
using DFDS.CapabilityService.WebApi.Domain.Repositories;

namespace DFDS.CapabilityService.Tests.Builders
{
    public class CapabilityApplicationServiceBuilder
    {
        private ICapabilityRepository _capabilityRepository;
        private ITopicRepository _topicRepository;

        public CapabilityApplicationServiceBuilder()
        {
            _capabilityRepository = Dummy.Of<ICapabilityRepository>();
            _topicRepository = Dummy.Of<ITopicRepository>();
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
       
        public CapabilityApplicationService Build()
        {
            return new CapabilityApplicationService(
                capabilityRepository: _capabilityRepository,
                topicRepository: _topicRepository
            );
        }
    }
}