using DFDS.CapabilityService.WebApi.Features.Shared.Infrastructure.Messaging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace DFDS.CapabilityService.Tests.Builders
{
    public class EventDispatcherBuilder
    {
        private ILogger<EventDispatcher> _logger;
        private DomainEventRegistry _eventRegistry;
        private EventHandlerFactory _eventHandlerFactory;
        
        public EventDispatcherBuilder()
        {
            _logger = new Logger<EventDispatcher>(new NullLoggerFactory());
            _eventRegistry = new DomainEventRegistry();
            _eventHandlerFactory = new EventHandlerFactory();
        }

        public EventDispatcherBuilder WithLogger(ILogger<EventDispatcher> logger)
        {
            _logger = logger;
            return this;
        }

        public EventDispatcherBuilder WithEventRegistry(DomainEventRegistry eventRegistry)
        {
            _eventRegistry = eventRegistry;
            return this;
        }
        
        public EventDispatcherBuilder WithEventHandlerFactory(EventHandlerFactory eventHandlerFactory)
        {
            _eventHandlerFactory = eventHandlerFactory;
            return this;
        }


        public EventDispatcher Build()
        {
            return new EventDispatcher(_logger, _eventRegistry, _eventHandlerFactory);
        }
    }
}