using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DFDS.CapabilityService.WebApi.Domain.Models;

namespace DFDS.CapabilityService.WebApi.Application
{
    public interface ICapabilityApplicationService
    {
        Task<Capability> CreateCapability(string name, string description);
        Task<Capability> UpdateCapability(Guid id, string newName, string newDescription);
        Task<IEnumerable<Capability>> GetAllCapabilities();
        Task<Capability> GetCapability(Guid id);
        Task DeleteCapability(Guid id);
        Task JoinCapability(Guid capabilityId, string memberEmail);
        Task LeaveCapability(Guid capabilityId, string memberEmail);
        Task AddContext(Guid capabilityId, string contextName);
        Task UpdateContext(Guid capabilityId, Guid contextId, string awsAccountId, string awsRoleArn, string awsRoleEmail);

        Task<IEnumerable<Topic>> GetTopicsForCapability(Guid capabilityId);
        Task AddTopic(Guid capabilityId, string topicName, string topicDescription, bool isTopicPrivate);
    }
}