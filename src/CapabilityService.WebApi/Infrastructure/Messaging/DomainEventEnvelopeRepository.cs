using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DFDS.CapabilityService.WebApi.Features.Capabilities.Infrastructure.Persistence;
using DFDS.CapabilityService.WebApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DFDS.CapabilityService.WebApi.Infrastructure.Messaging
{
    public class DomainEventEnvelopeRepository: IRepository<DomainEventEnvelope>
    {
        private readonly CapabilityServiceDbContext _dbContext;

        public DomainEventEnvelopeRepository(CapabilityServiceDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Add(IEnumerable<DomainEventEnvelope> domainEvents)
        {
            await _dbContext.DomainEvents.AddRangeAsync(domainEvents);
        }

        public async Task<IEnumerable<DomainEventEnvelope>> GetAll()
        {
            return await _dbContext.DomainEvents.ToListAsync();
        }
    }
}