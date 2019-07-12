using System;
using System.Threading.Tasks;
using DFDS.CapabilityService.WebApi.Domain.Events;
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
            var generalDomainEventObj = JsonConvert.DeserializeObject<GeneralDomainEvent>(generalDomainEventJson);
            await SendAsync(generalDomainEventObj, serviceScope);
        }

        public async Task SendAsync(GeneralDomainEvent generalDomainEvent, IServiceScope serviceScope)
        {
            var requestCorrelationHandler = serviceScope.ServiceProvider.GetRequiredService<IRequestCorrelation>();
            requestCorrelationHandler.OverrideCorrelationId(generalDomainEvent.XCorrelationId);

            using (LogContext.PushProperty("CorrelationId", requestCorrelationHandler.RequestCorrelationId))
            {
                var eventType = _eventRegistry.GetInstanceTypeFor(generalDomainEvent.EventName);
                dynamic domainEvent = Activator.CreateInstance(eventType, generalDomainEvent);
                dynamic handlersList = _eventHandlerFactory.GetEventHandlersFor(domainEvent, serviceScope);

                foreach (var handler in handlersList)
                {
                    await handler.HandleAsync(domainEvent);
                }
            }
        }
    }
}