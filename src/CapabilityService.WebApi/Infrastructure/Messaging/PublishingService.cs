using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using DFDS.CapabilityService.WebApi.Domain.Events;
using DFDS.CapabilityService.WebApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Serilog;

namespace DFDS.CapabilityService.WebApi.Infrastructure.Messaging
{
    public class PublishingService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public PublishingService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await DoWork(stoppingToken);
                await Task.Delay(TimeSpan.FromSeconds(20), stoppingToken);
            }
        }

        private async Task DoWork(CancellationToken stoppingToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<CapabilityServiceDbContext>();
                var domainEvents = await dbContext
                    .DomainEvents
                    .Where(x => x.Sent == null)
                    .ToListAsync(stoppingToken);
                
                Log.Information($"Domain events to publish: {domainEvents.Count}");

                var publisherFactory = scope.ServiceProvider.GetRequiredService<KafkaPublisherFactory>();
                var eventRegistry = scope.ServiceProvider.GetRequiredService<DomainEventRegistry>();

                Log.Information("Connecting to kafka...");
                
                using (var producer = publisherFactory.Create())
                {
                    Log.Information("Connected!");
                    
                    foreach (var evt in domainEvents)
                    {
                        var topicName = eventRegistry.GetTopicFor(evt.Type);
                        var message = MessagingHelper.CreateMessageFrom(evt);

                        var result = await producer.ProduceAsync(
                            topic: topicName,
                            key: evt.AggregateId,
                            val: message
                        );

                        if (!result.Error.HasError)
                        {
                            evt.Sent = result.Timestamp.UtcDateTime;
                            await dbContext.SaveChangesAsync();
                            Log.Information($"Domain event \"{evt.Type}>{evt.EventId}\" has been published!");
                        }
                        else
                        {
                            throw new Exception($"Could not publish domain event \"{evt.Type}>{evt.EventId}\"!!!");
                        }
                    }
                }
            }
        }
    }

    public static class MessagingHelper
    {
        public static string CreateMessageFrom(DomainEventEnvelope evt)
        {
            var domainEvent = JsonConvert.DeserializeObject<ExpandoObject>(evt.Data);
            var message = new
            {
                MessageId = evt.EventId,
                Type = evt.Type,
                Data = domainEvent
            };

            return JsonConvert.SerializeObject(message, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
        }
    }

    public class DomainEventRegistry
    {
        private readonly List<DomainEventRegistration> _registrations = new List<DomainEventRegistration>();

        public DomainEventRegistry Register<TEvent>(string eventTypeName, string topicName) where TEvent : IDomainEvent
        {
            _registrations.Add(new DomainEventRegistration
            {
                EventType = eventTypeName,
                EventInstanceType = typeof(TEvent),
                Topic = topicName
            });

            return this;
        }

        public string GetTopicFor(string eventType)
        {
            var registration = _registrations.SingleOrDefault(x => x.EventType == eventType);

            if (registration != null)
            {
                return registration.Topic;
            }

            return null;
        }

        public string GetTypeNameFor(IDomainEvent domainEvent)
        {
            var registration = _registrations.SingleOrDefault(x => x.EventInstanceType == domainEvent.GetType());

            if (registration == null)
            {
                throw new MessagingException($"Error! Could not determine \"event type name\" due to no registration was found for type {domainEvent.GetType().FullName}!");
            }

            return registration.EventType;
        }

        public class DomainEventRegistration
        {
            public string EventType { get; set; }
            public Type EventInstanceType { get; set; }
            public string Topic { get; set; }
        }
    }

    public class MessagingException : Exception
    {
        public MessagingException(string message) : base(message)
        {
            
        }
    }

    public class KafkaPublisherFactory
    {
        private readonly KafkaConfiguration _configuration;

        public KafkaPublisherFactory(KafkaConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Producer<string, string> Create()
        {
            var config = _configuration.AsEnumerable().ToArray();
            
            return new Producer<string, string>(
                config: config,
                keySerializer: new StringSerializer(Encoding.UTF8),
                valueSerializer: new StringSerializer(Encoding.UTF8)
            );
        }

        public class KafkaConfiguration
        {
            private const string KEY_PREFIX = "CAPABILITY_SERVICE_KAFKA_";
            private readonly IConfiguration _configuration;

            public KafkaConfiguration(IConfiguration configuration)
            {
                _configuration = configuration;
            }

            private string Key(string keyName) => string.Join("", KEY_PREFIX, keyName.ToUpper().Replace('.', '_'));

            private Tuple<string, string> GetConfiguration(string key)
            {
                var value = _configuration[Key(key)];

                if (string.IsNullOrWhiteSpace(value))
                {
                    return null;
                }

                return Tuple.Create<string, string>(key, value);
            }

            public IEnumerable<KeyValuePair<string, object>> AsEnumerable()
            {
                var configurationKeys = new[]
                {
                    "bootstrap.servers",
                    "broker.version.fallback",
                    "api.version.fallback.ms",
                    "ssl.ca.location",
                    "sasl.username",
                    "sasl.password",
                    "sasl.mechanisms",
                    "security.protocol",
                };

                var config = configurationKeys
                    .Select(key => GetConfiguration(key))
                    .Where(pair => pair != null)
                    .Select(pair => new KeyValuePair<string, object>(pair.Item1, pair.Item2))
                    .ToList();
                
                config.Add(new KeyValuePair<string, object>("request.timeout.ms", "3000"));

                return config;
            }
        }
    }
}