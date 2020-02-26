using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DFDS.CapabilityService.WebApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DFDS.CapabilityService.WebApi.Infrastructure.Messaging
{
    public class DomainEventEnvelopeRepository: IRepository<DomainEventEnvelope>
    {
        private readonly ICapabilityServiceDbContextFactory _capabilityServiceDbContextFactory;

        public DomainEventEnvelopeRepository(ICapabilityServiceDbContextFactory capabilityServiceDbContextFactory)
        {
	        _capabilityServiceDbContextFactory = capabilityServiceDbContextFactory;
        }

        public async Task Add(IEnumerable<DomainEventEnvelope> domainEvents)
        {
	        var dbContext = new CapabilityServiceDbContext(_capabilityServiceDbContextFactory.Create().Options);
	        await dbContext.DomainEvents.AddRangeAsync(domainEvents);
        }

        public async Task<IEnumerable<DomainEventEnvelope>> GetAll()
        {
	        var dbContext = new CapabilityServiceDbContext(_capabilityServiceDbContextFactory.Create().Options);
	        return await dbContext.DomainEvents.ToListAsync();
        }
    }
}
