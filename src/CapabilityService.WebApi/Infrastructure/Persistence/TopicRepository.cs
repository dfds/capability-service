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

        public async Task<IEnumerable<Topic>> GetAll()
        {
            return await _dbContext.Topics
                .Include(x => x.MessageContracts)
                .ToListAsync();
        }

        public async Task<IEnumerable<Topic>> GetByCapability(Guid capabilityId)
        {
            return await _dbContext.Topics
                .Where(x => x.CapabilityId == capabilityId)
                .Include(x => x.MessageContracts)
                .ToListAsync();
        }

        public async Task Add(Topic topic)
        {
            await _dbContext.Topics.AddAsync(topic);

            await _dbContext.SaveChangesAsync();
        }

        public async Task<Topic> Get(Guid id)
        {
            return await _dbContext.Topics
                .Include(x => x.MessageContracts)
                .SingleOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<Topic>> FindBy(string name)
        {
            var topics = await _dbContext.Topics
                .Include(x => x.MessageContracts)
                .Where(x => x.Name == name)
                .ToListAsync();

            return topics;
        }
    }
}
