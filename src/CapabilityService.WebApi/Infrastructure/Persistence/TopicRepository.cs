using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DFDS.CapabilityService.WebApi.Domain.Models;
using DFDS.CapabilityService.WebApi.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DFDS.CapabilityService.WebApi.Infrastructure.Persistence
{
    public class TopicRepository : ITopicRepository
    {
        private readonly CapabilityServiceDbContext _dbContext;

        public TopicRepository(CapabilityServiceDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Topic>> GetByCapability(Guid capabilityId)
        {
            return await _dbContext.Topics
                .Where(x => x.CapabilityId == capabilityId)
                .ToListAsync();
        }

        public async Task Add(Topic topic)
        {
            await _dbContext.Topics.AddAsync(topic);
        }
    }
}