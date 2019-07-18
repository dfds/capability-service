using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using DFDS.CapabilityService.WebApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
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
            Console.WriteLine("Starting event publisher.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await DoWork(stoppingToken);
                }
                catch (Exception err)
                {
                    Log.Error(err, "Error processing and/or publishing domain events to message broker.");
                }

                await Task.Delay(TimeSpan.FromSeconds(20), stoppingToken);
            }
        }

        private async Task DoWork(CancellationToken stoppingToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<CapabilityServiceDbContext>();
                var domainEventsToPublish = await dbContext
                    .DomainEvents
                    .Where(x => x.Sent == null)
                    .ToListAsync(stoppingToken);
                
                if (domainEventsToPublish.Any() == false)
                {
                    return;
                }
                
                Log.Information($"Domain events to publish: {domainEventsToPublish.Count}");

                var publisherFactory = scope.ServiceProvider.GetRequiredService<KafkaPublisherFactory>();
                var eventRegistry = scope.ServiceProvider.GetRequiredService<IDomainEventRegistry>();

                Log.Information("Connecting to kafka...");
                
                using (var producer = publisherFactory.Create())
                {
                    Log.Information("Connected!");
                    
                    foreach (var evt in domainEventsToPublish)
                    {
                        var topicName = eventRegistry.GetTopicFor(evt.Type);
                        var message = MessagingHelper.CreateMessageFrom(evt);

                        try
                        {
                            var result = await producer.ProduceAsync(
                                topic: topicName,
                                message: new Message<string, string>
                                {
                                    Key = evt.AggregateId,
                                    Value = message
                                }
                            );
                            
                            evt.Sent = result.Timestamp.UtcDateTime;
                            await dbContext.SaveChangesAsync();
                            Log.Information($"Domain event \"{evt.Type}>{evt.EventId}\" has been published!");
                        }
                        catch (Exception)
                        {
                            throw new Exception($"Could not publish domain event \"{evt.Type}>{evt.EventId}\"!!!");
                        }
                    }
                }
            }
        }
    }
}