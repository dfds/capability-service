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

    public abstract class TransactionalDecoratorBase
    {
        private readonly CapabilityServiceDbContext _dbContext;

        protected TransactionalDecoratorBase(CapabilityServiceDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        protected async Task ExecuteInTransaction(Func<Task> action)
        {
            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                await action();

                await _dbContext.SaveChangesAsync();
                transaction.Commit();
            }
        }

        protected async Task<T> ExecuteInTransaction<T>(Func<Task<T>> action)
        {
            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                var result = await action();

                await _dbContext.SaveChangesAsync();
                transaction.Commit();

                return result;
            }
        }
    }

    public class TopicTransactionalDecorator : TransactionalDecoratorBase, ITopicApplicationService
    {
        private readonly ITopicApplicationService _inner;

        public TopicTransactionalDecorator(ITopicApplicationService inner, CapabilityServiceDbContext dbContext) : base(dbContext)
        {
            _inner = inner;
        }

        public Task UpdateMessageContract(Guid topicId, string type, string description, string content)
        {
            return ExecuteInTransaction(() =>_inner.UpdateMessageContract(topicId, type, description, content));
        }

        public Task RemoveMessageContract(Guid topicId, string type)
        {
            return ExecuteInTransaction(() => _inner.RemoveMessageContract(topicId, type));
        }

        public Task UpdateTopic(Guid topicId, string name, string description, bool isPrivate)
        {
            return ExecuteInTransaction(() => _inner.UpdateTopic(topicId, name, description, isPrivate));
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

        public async Task<Capability> UpdateCapability(Guid capabilityId, string newName, string newDescription)
        {
            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                var cap = await _inner.UpdateCapability(capabilityId, newName, newDescription);

                await _dbContext.SaveChangesAsync();
                transaction.Commit();
                return cap;
            }        
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

        public Task<IEnumerable<Topic>> GetTopicsForCapability(Guid capabilityId) => _inner.GetTopicsForCapability(capabilityId);

        public async Task AddTopic(Guid capabilityId, string topicName, string topicDescription, bool isTopicPrivate)
        {
            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                await _inner.AddTopic(capabilityId, topicName, topicDescription, isTopicPrivate);

                await _dbContext.SaveChangesAsync();
                transaction.Commit();
            }
        }
    }
}