using DFDS.CapabilityService.Tests.TestDoubles;
using DFDS.CapabilityService.WebApi.Features.Shared.Infrastructure.Messaging;

namespace DFDS.CapabilityService.Tests.Builders
{
    public class DomainEventScannerBuilder
    {
        private IDomainEventRegistrationManager _domainEventRegistrationManager;

        public DomainEventScannerBuilder()
        {
            _domainEventRegistrationManager = Dummy.Of<IDomainEventRegistrationManager>();
        }

        public DomainEventScannerBuilder WithDomainEventRegistrationManager(IDomainEventRegistrationManager domainEventRegistrationManager)
        {
            _domainEventRegistrationManager = domainEventRegistrationManager;
            return this;
        }

        public DomainEventScanner Build()
        {
            return new DomainEventScanner(_domainEventRegistrationManager);
        }
    }
}