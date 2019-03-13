using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DFDS.CapabilityService.WebApi.Domain.Exceptions;
using DFDS.CapabilityService.WebApi.Domain.Models;
using DFDS.CapabilityService.WebApi.Domain.Repositories;
using DFDS.CapabilityService.WebApi.Infrastructure.Integrations;

namespace DFDS.CapabilityService.WebApi.Application
{
    public class CapabilityApplicationService : ICapabilityApplicationService
    {
        private readonly ICapabilityRepository _capabilityRepository;
        private readonly IRoleService _roleService;
        private readonly Regex _nameValidationRegex = new Regex("^[A-Z][a-zA-Z0-9_\\-]{2,30}$", RegexOptions.Compiled);

        public CapabilityApplicationService(ICapabilityRepository capabilityRepository, IRoleService roleService)
        {
            _capabilityRepository = capabilityRepository;
            _roleService = roleService;
        }

        public Task<Capability> GetCapability(Guid id) => _capabilityRepository.Get(id);
        public Task<IEnumerable<Capability>> GetAllCapabilities() => _capabilityRepository.GetAll();

        public async Task<Capability> CreateCapability(string name)
        {
            if (!_nameValidationRegex.Match(name).Success)
            {
                throw new CapabilityValidationException("Name must be a string of length 3 to 32. consisting of only alphanumeric ASCII characters, starting with a capital letter. Underscores and hyphens are allowed.");
            }

            var capability = Capability.Create(name);
            await _capabilityRepository.Add(capability);
            await _roleService.CreateRoleFor(capability);

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
    }
}