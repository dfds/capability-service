using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DFDS.CapabilityService.WebApi.Application;
using DFDS.CapabilityService.WebApi.Domain.Models;

namespace DFDS.CapabilityService.Tests.TestDoubles
{
    public class StubCapabilityApplicationService : ICapabilityApplicationService
    {
        private readonly Capability[] _stubCapabilities;

        public StubCapabilityApplicationService(params Capability[] stubCapabilities)
        {
            _stubCapabilities = stubCapabilities;
        }

        public Task<Capability> CreateCapability(string name, string description)
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
            return Task.FromResult(capability);
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

        public Task<IEnumerable<Capability>> GetAllCapabilities()
        {
            throw _exceptionToThrow;
        }

        public Task<Capability> GetCapability(Guid id)
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
    }

}