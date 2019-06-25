using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DFDS.CapabilityService.WebApi.Domain.Exceptions;
using DFDS.CapabilityService.WebApi.Domain.Models;
using DFDS.CapabilityService.WebApi.Domain.Repositories;

namespace DFDS.CapabilityService.WebApi.Application
{
    public class CapabilityApplicationService : ICapabilityApplicationService
    {
        private readonly ICapabilityRepository _capabilityRepository;
        private readonly Regex _nameValidationRegex = new Regex("^[A-Z][a-zA-Z0-9_\\-]{2,30}$", RegexOptions.Compiled);

        public CapabilityApplicationService(ICapabilityRepository capabilityRepository)
        {
            _capabilityRepository = capabilityRepository;
        }

        public Task<Capability> GetCapability(Guid id) => _capabilityRepository.Get(id);
        public Task<IEnumerable<Capability>> GetAllCapabilities() => _capabilityRepository.GetAll();

        public async Task<Capability> CreateCapability(string name, string description)
        {
            if (!_nameValidationRegex.Match(name).Success)
            {
                throw new CapabilityValidationException("Name must be a string of length 3 to 32. consisting of only alphanumeric ASCII characters, starting with a capital letter. Underscores and hyphens are allowed.");
            }

            var capability = Capability.Create(name, description);
            await _capabilityRepository.Add(capability);

            return capability;
        }

        public async Task JoinCapability(Guid capabilityId, string memberEmail)
        {
            var capability = await _capabilityRepository.Get(capabilityId);

            if (capability == null)
            {
                throw new CapabilityDoesNotExistException();
            }

            capability.StartMembershipFor(memberEmail);
        }

        public async Task LeaveCapability(Guid capabilityId, string memberEmail)
        {
            var capability = await _capabilityRepository.Get(capabilityId);

            if (capability == null)
            {
                throw new CapabilityDoesNotExistException();
            }

            capability.StopMembershipFor(memberEmail);
        }

        public async Task AddContext(Guid capabilityId, string contextName)
        {
            var capability = await _capabilityRepository.Get(capabilityId);

            if (capability == null)
            {
                throw new CapabilityDoesNotExistException();
            }
            
            capability.AddContext(contextName);
        }

        public async Task UpdateContext(Guid capabilityId, Guid contextId, string awsAccountId, string awsRoleArn, string awsRoleEmail)
        {
            var capability = await _capabilityRepository.Get(capabilityId);
            if (capability == null)
            {
                throw new CapabilityDoesNotExistException();
            }

            var context = capability.Contexts.FirstOrDefault(c => c.Id == contextId);
            if (context == null)
            {
                throw new ContextDoesNotExistException();
            }
            capability.UpdateContext(context.Id, awsAccountId, awsRoleArn, awsRoleEmail);
        }
    }
}