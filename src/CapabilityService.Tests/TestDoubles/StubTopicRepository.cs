using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DFDS.CapabilityService.WebApi.Domain.Models;
using DFDS.CapabilityService.WebApi.Domain.Repositories;

namespace DFDS.CapabilityService.Tests.TestDoubles
{
    public class StubTopicRepository : ITopicRepository
    {
        private readonly Topic[] _topics;

        public StubTopicRepository(params Topic[] topics)
        {
            _topics = topics;
        }

        public Task<IEnumerable<Topic>> GetAll()
        {
            return Task.FromResult(_topics.AsEnumerable());
        }

        public Task<IEnumerable<Topic>> GetByCapability(Guid capabilityId)
        {
            return Task.FromResult(_topics.AsEnumerable());
        }

        public Task Add(Topic topic)
        {
            throw new NotImplementedException();
        }

        public Task<Topic> Get(Guid id)
        {
            return Task.FromResult(_topics.FirstOrDefault());
        }
    }
}