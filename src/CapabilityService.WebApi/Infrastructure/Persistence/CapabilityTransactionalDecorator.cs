using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DFDS.CapabilityService.WebApi.Application;
using DFDS.CapabilityService.WebApi.Domain.Models;
using DFDS.CapabilityService.WebApi.Infrastructure.Messaging;

namespace DFDS.CapabilityService.WebApi.Infrastructure.Persistence
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

        public async Task<Capability> CreateCapability(string name, string description)
        {
            var capability = await _inner.CreateCapability(name, description);
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
    }


    public class CapabilityTransactionalDecorator : ICapabilityApplicationService
    {
        private readonly ICapabilityApplicationService _inner;
        private readonly CapabilityServiceDbContext _dbContext;

        public CapabilityTransactionalDecorator(ICapabilityApplicationService inner, CapabilityServiceDbContext dbContext)
        {
            _inner = inner;
            _dbContext = dbContext;
        }

        public Task<IEnumerable<Capability>> GetAllCapabilities() => _inner.GetAllCapabilities();
        public Task<Capability> GetCapability(Guid id) => _inner.GetCapability(id);

        public async Task<Capability> CreateCapability(string name, string description)
        {
            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                var capability = await _inner.CreateCapability(name, description);

                await _dbContext.SaveChangesAsync();
                transaction.Commit();

                return capability;
            }
        }

        public async Task JoinCapability(Guid capabilityId, string memberEmail)
        {
            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                await _inner.JoinCapability(capabilityId, memberEmail);

                await _dbContext.SaveChangesAsync();
                transaction.Commit();
            }
        }

        public async Task LeaveCapability(Guid capabilityId, string memberEmail)
        {
            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                await _inner.LeaveCapability(capabilityId, memberEmail);

                await _dbContext.SaveChangesAsync();
                transaction.Commit();
            }
        }

        public async Task AddContext(Guid capabilityId, string contextName)
        {
            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                await _inner.AddContext(capabilityId, contextName);

                await _dbContext.SaveChangesAsync();
                transaction.Commit();
            }
        }

        public async Task UpdateContext(Guid capabilityId, Guid contextId, string awsAccountId, string awsRoleArn, string awsRoleEmail)
        {
            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                await _inner.UpdateContext(capabilityId, contextId, awsAccountId, awsRoleArn, awsRoleEmail);

                await _dbContext.SaveChangesAsync();
                transaction.Commit();
            }
        }
    }
}