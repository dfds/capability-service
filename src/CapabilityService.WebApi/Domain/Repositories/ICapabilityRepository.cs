using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DFDS.CapabilityService.WebApi.Domain.Models;

namespace DFDS.CapabilityService.WebApi.Domain.Repositories
{
    public interface ICapabilityRepository
    {
        Task<IEnumerable<Capability>> GetAll();
        Task Add(Capability capability);
        Task<Capability> Get(Guid id);
        Task<Capability> GetByRootId(string capabilityRootId);
    }
}
