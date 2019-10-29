using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DFDS.CapabilityService.WebApi.Features.Capabilities.Domain.Models;

namespace DFDS.CapabilityService.WebApi.Features.Capabilities.Domain.Repositories
{
    public interface ICapabilityRepository
    {
        Task<IEnumerable<Capability>> GetAll();
        Task Add(Capability capability);
        Task<Capability> Get(Guid id);

        void Remove(Capability capability);
    }
}