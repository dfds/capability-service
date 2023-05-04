using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DFDS.CapabilityService.WebApi.Application;
using DFDS.CapabilityService.WebApi.Domain.Exceptions;
using DFDS.CapabilityService.WebApi.Domain.Models;

namespace DFDS.CapabilityService.Tests.TestDoubles
{
    public class StubCapabilityApplicationService : ICapabilityApplicationService
    {
        private readonly List<Capability> _stubCapabilities;

        public StubCapabilityApplicationService(Capability[] stubCapabilities = null)
        {
            _stubCapabilities = stubCapabilities?.ToList() ?? new List<Capability>();
        }

        public Task<Capability> CreateCapability(string name, string description)
        {
            var capability = _stubCapabilities.FirstOrDefault();
            return Task.FromResult(capability);
        }

        public Task<Capability> UpdateCapability(Guid id, string newName, string newDescription)
        {
            var capability = _stubCapabilities.FirstOrDefault();
            return Task.FromResult(capability);
        }

        public Task<IEnumerable<Capability>> GetAllCapabilities()
        {
            return Task.FromResult(_stubCapabilities.AsEnumerable());
        }

        public Task<Capability> GetCapability(Guid id)
        {
            var capability = _stubCapabilities.FirstOrDefault();

            if (capability == null)
            {
	            throw new CapabilityDoesNotExistException(id);
            }
            return Task.FromResult(capability);
        }

        public Task DeleteCapability(Guid id)
        {
            var capabilityToRemove = _stubCapabilities.Single(c => c.Id == id);
            _stubCapabilities.Remove(capabilityToRemove);
            
            return Task.CompletedTask;
        }

        public Task JoinCapability(Guid capabilityId, string memberEmail)
        {
            return Task.CompletedTask;
        }

        public Task LeaveCapability(Guid capabilityId, string memberEmail)
        {
            return Task.CompletedTask;
        }

        public Task AddContext(Guid capabilityId, string contextName)
        {
            return Task.CompletedTask;
        }

        public Task UpdateContext(string capabilityId, string contextId, string awsAccountId, string awsRoleArn, string awsRoleEmail)
        {
            return Task.CompletedTask;
        }
    }

    public class ErroneousCapabilityApplicationService : ICapabilityApplicationService
    {
        private readonly Exception _exceptionToThrow;

        public ErroneousCapabilityApplicationService(Exception exceptionToThrow)
        {
            _exceptionToThrow = exceptionToThrow;
        }

        public Task<Capability> CreateCapability(string name, string description)
        {
            throw _exceptionToThrow;
        }

        public Task<Capability> UpdateCapability(Guid id, string newName, string newDescription)
        {
            throw _exceptionToThrow;
        }

        public Task<IEnumerable<Capability>> GetAllCapabilities()
        {
            throw _exceptionToThrow;
        }

        public Task<Capability> GetCapability(Guid id)
        {
            throw _exceptionToThrow;
        }

        public Task DeleteCapability(Guid id)
        {
            throw _exceptionToThrow;
        }

        public Task JoinCapability(Guid capabilityId, string memberEmail)
        {
            throw _exceptionToThrow;
        }

        public Task LeaveCapability(Guid capabilityId, string memberEmail)
        {
            throw _exceptionToThrow;
        }

        public Task AddContext(Guid capabilityId, string contextName)
        {
            throw _exceptionToThrow;
        }

        public Task UpdateContext(string capabilityId, string contextId, string awsAccountId, string awsRoleArn, string awsRoleEmail)
        {
            throw _exceptionToThrow;
        }
    }
}
