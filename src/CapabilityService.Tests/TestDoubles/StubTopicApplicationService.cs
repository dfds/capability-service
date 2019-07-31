using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DFDS.CapabilityService.WebApi.Application;
using DFDS.CapabilityService.WebApi.Domain.Models;

namespace DFDS.CapabilityService.Tests.TestDoubles
{
    public class StubTopicApplicationService : ITopicApplicationService
    {
        private readonly Topic[] _topics;

        public StubTopicApplicationService(params Topic[] topics)
        {
            _topics = topics;
        }

        public Task<IEnumerable<Topic>> GetAllTopics(Guid capabilityId)
        {
            return Task.FromResult(_topics.AsEnumerable());
        }
    }
}