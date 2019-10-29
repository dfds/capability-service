using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DFDS.CapabilityService.WebApi.Features.Capabilities.Application;
using DFDS.CapabilityService.WebApi.Features.Capabilities.Domain.Models;
using DFDS.CapabilityService.WebApi.Features.Shared.Domain.Models;
using DFDS.CapabilityService.WebApi.Features.Shared.Infrastructure.Messaging;
using DFDS.CapabilityService.WebApi.Features.Shared.Infrastructure.Persistence;
using DFDS.CapabilityService.WebApi.Features.Topics.Domain.Models;

namespace DFDS.CapabilityService.WebApi.Features.Capabilities.Infrastructure.Persistence
{
    public class CapabilityOutboxEnabledDecorator : ICapabilityApplicationService
    {
        private readonly ICapabilityApplicationService _inner;
        private readonly CapabilityServiceDbContext _dbContext;
        private readonly Outbox _outbox;

        public CapabilityOutboxEnabledDecorator(ICapabilityApplicationService inner, CapabilityServiceDbContext dbContext, Outbox outbox)
        {
            _inner = inner;
            _dbContext = dbContext;
            _outbox = outbox;
        }

        public Task<IEnumerable<Capability>> GetAllCapabilities() => _inner.GetAllCapabilities();
        public Task<Capability> GetCapability(Guid id) => _inner.GetCapability(id);
        public async Task DeleteCapability(Guid id)
        {
            await _inner.DeleteCapability(id);
            await QueueDomainEvents();
        }

        public async Task<Capability> CreateCapability(string name, string description)
        {
            var capability = await _inner.CreateCapability(name, description);
            await QueueDomainEvents();

            return capability;
        }
        
        public async Task<Capability> UpdateCapability(Guid id, string newName, string newDescription)
        {
            var capability = await _inner.UpdateCapability(id, newName, newDescription);
            await QueueDomainEvents();

            return capability;
        }        

        private async Task QueueDomainEvents()
        {
            var aggregates = _dbContext
                .ChangeTracker
                .Entries<IAggregateDomainEvents>()
                .Select(x => x.Entity)
                .ToArray();

            foreach (var aggregate in aggregates)
            {
                await _outbox.QueueDomainEvents(aggregate);
                aggregate.ClearDomainEvents();
            }
        }

        public async Task JoinCapability(Guid capabilityId, string memberEmail)
        {
            await _inner.JoinCapability(capabilityId, memberEmail);
            await QueueDomainEvents();
        }

        public async Task LeaveCapability(Guid capabilityId, string memberEmail)
        {
            await _inner.LeaveCapability(capabilityId, memberEmail);
            await QueueDomainEvents();
        }

        public async Task AddContext(Guid capabilityId, string contextName)
        {
            await _inner.AddContext(capabilityId, contextName);
            await QueueDomainEvents();
        }

        public async Task UpdateContext(Guid capabilityId, Guid contextId, string awsAccountId, string awsRoleArn, string awsRoleEmail)
        {
            await _inner.UpdateContext(capabilityId, contextId, awsAccountId, awsRoleArn, awsRoleEmail);
            await QueueDomainEvents();
        }

        public Task<IEnumerable<Topic>> GetTopicsForCapability(Guid capabilityId) => _inner.GetTopicsForCapability(capabilityId);

        public async Task AddTopic(Guid capabilityId, string topicName, string topicDescription, bool isTopicPrivate)
        {
            await _inner.AddTopic(capabilityId, topicName, topicDescription, isTopicPrivate);
            await QueueDomainEvents();
        }
    }
}