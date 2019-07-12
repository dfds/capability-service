using CorrelationId;
using DFDS.CapabilityService.WebApi.Infrastructure.Messaging;

namespace DFDS.CapabilityService.Tests.Builders
{
    public class OutboxBuilder
    {
        private IRequestCorrelation _requestCorrelation;
        private IRepository<DomainEventEnvelope> _repository;
        private DomainEventRegistry _domainEventRegistry;

        public OutboxBuilder WithRequestCorrelation()
        {
            _requestCorrelation = new RequestCorrelationStub();
            return this;
        }

        public OutboxBuilder WithRepository(IRepository<DomainEventEnvelope> repository)
        {
            _repository = repository;
            return this;
        }
        
        public Outbox Build()
        {
            return new Outbox(_repository, _domainEventRegistry, _requestCorrelation);
        }

        public OutboxBuilder WithRegistry(DomainEventRegistry domainEventRegistry)
        {
            _domainEventRegistry = domainEventRegistry;
            return this;
        }
    }

    public class RequestCorrelationStub : IRequestCorrelation
    {
        public string RequestCorrelationId => "CORRELATION_STUB";
        public void OverrideCorrelationId(string _)
        {
            
        }
    }
}