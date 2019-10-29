using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DFDS.CapabilityService.WebApi.Features.Capabilities.Domain.Exceptions;
using DFDS.CapabilityService.WebApi.Features.Capabilities.Domain.Models;
using DFDS.CapabilityService.WebApi.Features.Capabilities.Domain.Repositories;
using DFDS.CapabilityService.WebApi.Features.Topics.Domain.Exceptions;
using DFDS.CapabilityService.WebApi.Features.Topics.Domain.Models;
using DFDS.CapabilityService.WebApi.Features.Topics.Domain.Repositories;

namespace DFDS.CapabilityService.WebApi.Features.Capabilities.Application
{
    public class CapabilityApplicationService : ICapabilityApplicationService
    {
        private readonly ICapabilityRepository _capabilityRepository;
        private readonly ITopicRepository _topicRepository;

        public CapabilityApplicationService(ICapabilityRepository capabilityRepository,
            ITopicRepository topicRepository)
        {
            _capabilityRepository = capabilityRepository;
            _topicRepository = topicRepository;
        }

        public Task<Capability> GetCapability(Guid id) => _capabilityRepository.Get(id);

        public async Task DeleteCapability(Guid id)
        {
            var capability = await _capabilityRepository.Get(id);
            capability.Delete();
            _capabilityRepository.Remove(capability);
        }


        public async Task<Capability> UpdateCapability(Guid id, string newName, string newDescription)
        {
            Capability.Create(newName, newDescription);


            var capability = await _capabilityRepository.Get(id);
            capability.UpdateInfoFields(newName, newDescription);

            return capability;
        }

        public Task<IEnumerable<Capability>> GetAllCapabilities() => _capabilityRepository.GetAll();

        public async Task<Capability> CreateCapability(string name, string description)
        {
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

        public async Task UpdateContext(Guid capabilityId, Guid contextId, string awsAccountId, string awsRoleArn,
            string awsRoleEmail)
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

        public async Task<IEnumerable<Topic>> GetTopicsForCapability(Guid capabilityId) =>
            await _topicRepository.GetByCapability(capabilityId);

        public async Task AddTopic(Guid capabilityId, string topicName, string topicDescription, bool isTopicPrivate)
        {
            var capability = await _capabilityRepository.Get(capabilityId);
            if (capability == null)
            {
                throw new CapabilityDoesNotExistException();
            }

            var existingTopicsWithSameName = await _topicRepository.FindBy(topicName);
            if (existingTopicsWithSameName.Any())
            {
                throw new TopicAlreadyExistException($"A topic with the name \"{topicName}\" already exist.");
            }

            var topic = capability.AddTopic(topicName, topicDescription, isTopicPrivate);
            await _topicRepository.Add(topic);
        }
    }
}