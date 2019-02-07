using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DFDS.CapabilityService.WebApi.Models
{
    public interface ICapabilityRepository
    {
        Task<IEnumerable<Capability>> GetAll();
        Task Add(Capability capability);
        Task<Capability> Get(Guid id);
    }
}