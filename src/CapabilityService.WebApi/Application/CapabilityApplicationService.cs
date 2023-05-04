using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DFDS.CapabilityService.WebApi.Domain.Exceptions;
using DFDS.CapabilityService.WebApi.Domain.Factories;
using DFDS.CapabilityService.WebApi.Domain.Models;
using DFDS.CapabilityService.WebApi.Domain.Repositories;

namespace DFDS.CapabilityService.WebApi.Application
{
    public class CapabilityApplicationService : ICapabilityApplicationService
    {
        private readonly ICapabilityRepository _capabilityRepository;
        private readonly ICapabilityFactory _capabilityFactory;
        public CapabilityApplicationService(
            ICapabilityRepository capabilityRepository,
            ICapabilityFactory capabilityFactory
        )
        {
            _capabilityRepository = capabilityRepository;
            _capabilityFactory = capabilityFactory;
        }

        public Task<Capability> GetCapability(Guid id) => _capabilityRepository.Get(id);

        public async Task DeleteCapability(Guid id)
        {
            var capability = await _capabilityRepository.Get(id);
            capability.Delete();
        }


        public async Task<Capability> UpdateCapability(Guid id, string newName, string newDescription)
        {
	        var capability = await _capabilityRepository.Get(id);

	        var name = string.IsNullOrWhiteSpace(newName)
		        ? new CapabilityName(capability.Name)
		        : new CapabilityName(newName);
	        
	        capability.UpdateInfoFields(name, newDescription);

            return capability;
        }

        public Task<IEnumerable<Capability>> GetAllCapabilities() => _capabilityRepository.GetAll();

        public async Task<Capability> CreateCapability(string name, string description)
        {
            var capability = await _capabilityFactory.Create(name, description);
            await _capabilityRepository.Add(capability);

            return capability;
        }

        public async Task JoinCapability(Guid capabilityId, string memberEmail)
        {
            var capability = await _capabilityRepository.Get(capabilityId);

            capability.StartMembershipFor(memberEmail);
        }

        public async Task LeaveCapability(Guid capabilityId, string memberEmail)
        {
            var capability = await _capabilityRepository.Get(capabilityId);

            capability.StopMembershipFor(memberEmail);
        }

        public async Task AddContext(Guid capabilityId, string contextName)
        {
            var capability = await _capabilityRepository.Get(capabilityId);

            capability.AddContext(contextName);
        }

        public async Task UpdateContext(string capabilityId,
          string contextId,
          string awsAccountId,
          string awsRoleArn,
          string awsRoleEmail)
        {

          if (!Guid.TryParse(capabilityId, out var typedCapabilityId))
          {
            // ignore - does not exists as it originated from the new selfservice-api
            return;
          }
          if (!Guid.TryParse(contextId, out var typedContextId))
          {
            // ignore - does not exists as it originated from the new selfservice-api
            return;
          }

          var capability = await _capabilityRepository.Get(typedCapabilityId);

          var context = capability.Contexts.FirstOrDefault(c => c.Id == typedContextId);
          if (context == null)
          {
            throw new ContextDoesNotExistException();
          }

          capability.UpdateContext(context.Id, awsAccountId, awsRoleArn, awsRoleEmail);
        }
    }
}
