using DFDS.CapabilityService.WebApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace DFDS.CapabilityService.Tests.Infrastructure.Persistence
{
	public class CapabilityServiceDbContextInMemory : ICapabilityServiceDbContextFactory
	{
		private readonly string _id;

		public CapabilityServiceDbContextInMemory(string id)
		{
			_id = id;
		}
		
		public DbContextOptionsBuilder<CapabilityServiceDbContext> Create()
		{
			var options = new DbContextOptionsBuilder<CapabilityServiceDbContext>();
			options.UseInMemoryDatabase(_id);
			options.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));
			return options;
		}
	}
}
