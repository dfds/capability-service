using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DFDS.CapabilityService.WebApi.Domain.Models;

namespace DFDS.CapabilityService.WebApi.Application
{
    public interface ICapabilityApplicationService
    {
        Task<Capability> CreateCapability(string name);
        Task<IEnumerable<Capability>> GetAllCapabilities();
        Task<Capability> GetCapability(Guid id);
        Task JoinCapability(Guid capabilityId, string memberEmail);
        Task LeaveCapability(Guid capabilityId, string memberEmail);
    }
}