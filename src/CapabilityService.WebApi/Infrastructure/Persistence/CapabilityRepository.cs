﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DFDS.CapabilityService.WebApi.Domain.Exceptions;
using DFDS.CapabilityService.WebApi.Domain.Models;
using DFDS.CapabilityService.WebApi.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DFDS.CapabilityService.WebApi.Infrastructure.Persistence
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
                         .Where(c => c.Deleted == null)
                         .OrderBy(x => x.Name)
                         .Include(x => x.Memberships)
                         .Include(x => x.Contexts)
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
                .Include(x => x.Contexts)
                .SingleOrDefaultAsync(x => x.Id == id);
            
            if (capability == null)
            {
	            throw new CapabilityDoesNotExistException(id);
            }
            
            return capability;
        }

        public async Task<Capability> GetByRootId(string capabilityRootId)
        {
	        return await _dbContext
		        .Capabilities
		        .Include(x => x.Memberships)
		        .Include(x => x.Contexts)
		        .SingleOrDefaultAsync(x => x.RootId==capabilityRootId);
        }
    }
}
