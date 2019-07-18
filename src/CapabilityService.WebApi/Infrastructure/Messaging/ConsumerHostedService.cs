using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DFDS.CapabilityService.WebApi.Infrastructure.Messaging
{
    public class ConsumerHostedService : IHostedService
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly ILogger<ConsumerHostedService> _logger;
        private readonly KafkaConsumerFactory _consumerFactory;
        private readonly IServiceProvider _serviceProvider;
        private readonly IDomainEventRegistry _eventRegistry;

        private Task _executingTask;

        public ConsumerHostedService(
            ILogger<ConsumerHostedService> logger,
            IServiceProvider serviceProvider,
            KafkaConsumerFactory kafkaConsumerFactory,
            IDomainEventRegistry domainEventRegistry)
        {

            _logger = logger;
            _consumerFactory = kafkaConsumerFactory;
            _serviceProvider = serviceProvider;
            _eventRegistry = domainEventRegistry;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Starting event consumer.");

            _executingTask = Task.Factory.StartNew(async () =>
                {
                    using (var consumer = _consumerFactory.Create())
                    {
                        var topics = _eventRegistry.GetAllTopics();

                        _logger.LogInformation($"Event consumer started. Listening to topics: {string.Join(",", topics)}");
                                                
                        consumer.Subscribe(topics);
                        
                        // consume loop
                        while (!_cancellationTokenSource.IsCancellationRequested)
                        {
                            ConsumeResult<string, string> msg;
                            try
                            {
                                msg = consumer.Consume(cancellationToken);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError($"Consumption of event failed, reason: {ex}");
                                continue;
                            }
                            
                            using (var scope = _serviceProvider.CreateScope())
                            {
                                _logger.LogInformation($"Received event: Topic: {msg.Topic} Partition: {msg.Partition}, Offset: {msg.Offset} {msg.Value}");

                                try
                                {
                                    var eventDispatcher =
                                        scope.ServiceProvider.GetRequiredService<IEventDispatcher>();
                                    await eventDispatcher.Send(msg.Value, scope);
                                    await Task.Run(() => consumer.Commit(msg));
                                }
                                catch (MessagingHandlerNotAvailable mhnae)
                                {
                                    _logger.LogWarning(mhnae,
                                        "Encountered a message {EventType} with no properly defined handler. A generic handler should be implemented. Skipping.",
                                        mhnae.EventType);
                                    await Task.Run(() => consumer.Commit(msg));
                                }
                                catch (MessagingMessageIncomprehensible mmi)
                                {
                                    _logger.LogWarning(mmi,
                                        "Encountered a message that was irrecoverably incomprehensible. Skipping. Raw message included {@Message} with value '{@MessageValue}'",
                                        msg, msg.Value);
                                    await Task.Run(() => consumer.Commit(msg));
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError(ex, "Error consuming event. Exception message: {ExceptionMessage}. Raw message included {@Message} with value '{@MessageValue}'",
                                        ex.Message, msg, msg.Value);
                                    // Do not commit the message, instead halt and wait
                                }
                            }
                        }
                    }
                }, _cancellationTokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default)
                .ContinueWith(task =>
                {
                    if (task.IsFaulted)
                    {
                        _logger.LogError(task.Exception, "Event loop crashed");
                    }
                }, cancellationToken);

            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            try
            {
                _cancellationTokenSource.Cancel();
            }
            finally
            {
                await Task.WhenAny(_executingTask, Task.Delay(-1, cancellationToken));
            }

            _cancellationTokenSource.Dispose();
        }
    }
}