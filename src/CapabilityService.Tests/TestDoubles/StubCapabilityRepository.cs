using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DFDS.CapabilityService.WebApi.Features.Capabilities.Domain.Models;
using DFDS.CapabilityService.WebApi.Features.Capabilities.Domain.Repositories;

namespace DFDS.CapabilityService.Tests.TestDoubles
{
    public class StubCapabilityRepository : ICapabilityRepository
    {
        private readonly List<Capability> _capabilities;

        public StubCapabilityRepository(params Capability[] capabilities)
        {
            _capabilities = capabilities?.ToList() ?? new List<Capability>();
        }

        public Task<IEnumerable<Capability>> GetAll()
        {
            return Task.FromResult(_capabilities.AsEnumerable());
        }

        public Task Add(Capability capability)
        {
            return Task.CompletedTask;
        }

        public Task<Capability> Get(Guid id)
        {
            var capability = _capabilities.FirstOrDefault();
            return Task.FromResult(capability);
        }

        public void Remove(Capability capability)
        {
            _capabilities.Remove(capability);
        }
    }
}