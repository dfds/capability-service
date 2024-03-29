using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DFDS.CapabilityService.WebApi.Application;
using DFDS.CapabilityService.WebApi.Domain.Models;

namespace DFDS.CapabilityService.WebApi.Infrastructure.Persistence
{
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
        public async Task DeleteCapability(Guid id)
        {
            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                await _inner.DeleteCapability(id);

                await _dbContext.SaveChangesAsync();
                transaction.Commit();
            }
        }

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

        public async Task UpdateContext(string capabilityId, string contextId, string awsAccountId, string awsRoleArn, string awsRoleEmail)
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
