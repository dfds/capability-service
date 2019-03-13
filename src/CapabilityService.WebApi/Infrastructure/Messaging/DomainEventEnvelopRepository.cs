using System.Collections.Generic;
using System.Threading.Tasks;
using DFDS.CapabilityService.WebApi.Infrastructure.Persistence;

namespace DFDS.CapabilityService.WebApi.Infrastructure.Messaging
{
    public class DomainEventEnvelopRepository
    {
        private readonly CapabilityServiceDbContext _dbContext;

        public DomainEventEnvelopRepository(CapabilityServiceDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Add(IEnumerable<DomainEventEnvelope> domainEvents)
        {
            await _dbContext.DomainEvents.AddRangeAsync(domainEvents);
        }
    }
}