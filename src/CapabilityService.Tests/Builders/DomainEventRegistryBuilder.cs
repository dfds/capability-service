using DFDS.CapabilityService.WebApi.Features.Shared.Infrastructure.Messaging;

namespace DFDS.CapabilityService.Tests.Builders
{
    public class DomainEventRegistryBuilder
    {
        public DomainEventRegistry Build()
        {
            return new DomainEventRegistry();
        }
    }
}