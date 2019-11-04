using System;
using System.Threading.Tasks;
using DFDS.CapabilityService.WebApi.Infrastructure.Events;
using DFDS.CapabilityService.WebApi.Domain.Events;
using DFDS.CapabilityService.WebApi.Enablers.CorrelationId;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog.Context;

namespace DFDS.CapabilityService.WebApi.Infrastructure.Messaging
{
    public class EventDispatcher : IEventDispatcher
    {
        private readonly ILogger<EventDispatcher> _logger;
        private readonly DomainEventRegistry _eventRegistry;
        private readonly EventHandlerFactory _eventHandlerFactory;

        public EventDispatcher(
            ILogger<EventDispatcher> logger,
            DomainEventRegistry eventRegistry,
            EventHandlerFactory eventHandlerFactory)
        {
            _logger = logger;
            _eventRegistry = eventRegistry;
            _eventHandlerFactory = eventHandlerFactory;
        }

        public async Task Send(string generalDomainEventJson, IServiceScope serviceScope)
        {
            try
            {
                var generalDomainEventObj = JsonConvert.DeserializeObject<GeneralIntegrationEvent>(generalDomainEventJson);
                await SendAsync(generalDomainEventObj, serviceScope);
            }
            catch (JsonReaderException ex)
            {
                throw new MessagingMessageIncomprehensible($"Was unable to deserialize from JSON to GeneralDomainEvent: {ex.Message}");
            }
        }

        public async Task SendAsync(GeneralIntegrationEvent generalIntegrationEvent, IServiceScope serviceScope)
        {
            if (generalIntegrationEvent == null)
            {
                throw new MessagingMessageIncomprehensible("Received a blank message");
            }
            var requestCorrelationHandler = serviceScope.ServiceProvider.GetRequiredService<IRequestCorrelation>();
            requestCorrelationHandler.OverrideCorrelationId(generalIntegrationEvent.XCorrelationId);

            using (LogContext.PushProperty("CorrelationId", generalIntegrationEvent.XCorrelationId))
            {
                var eventType = _eventRegistry.GetInstanceTypeFor(generalIntegrationEvent.EventName);
                try
                {
                    dynamic domainEvent = Activator.CreateInstance(eventType, generalIntegrationEvent);
                    dynamic handlersList = _eventHandlerFactory.GetEventHandlersFor(domainEvent, serviceScope);

                    foreach (var handler in handlersList)
                    {
                        await handler.HandleAsync(domainEvent);
                    }
                }
                catch (MissingMethodException mme)
                {
                    throw new MessagingHandlerNotAvailable($"Handler not available, specific error {mme.Message}", generalIntegrationEvent.EventName, mme);
                }    
            }
        }
    }
}