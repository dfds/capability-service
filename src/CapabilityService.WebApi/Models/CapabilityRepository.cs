using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DFDS.CapabilityService.WebApi.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DFDS.CapabilityService.WebApi.Models
{
    public class CapabilityRepository : ICapabilityRepository
    {
        private readonly CapabilityServiceDbContext _dbContext;

        public CapabilityRepository(CapabilityServiceDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Capability>> GetAll()
        {
            return await _dbContext
                         .Capabilities
                         .OrderBy(x => x.Name)
                         .Include(x => x.Memberships)
                         .ToListAsync();
        }

        public async Task Add(Capability capability)
        {
            await _dbContext.Capabilities.AddAsync(capability);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Capability> Get(Guid id)
        {
            var capability = await _dbContext
                .Capabilities
                .Include(x => x.Memberships)
                .SingleOrDefaultAsync(x => x.Id == id);
            return capability;
        }
    }
}