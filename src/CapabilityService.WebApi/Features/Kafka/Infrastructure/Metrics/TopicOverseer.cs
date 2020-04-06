using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Services;
using KafkaJanitor.RestClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace DFDS.CapabilityService.WebApi.Features.Kafka.Infrastructure.Metrics
{
	public class TopicOverseer : BackgroundService
	{
		private readonly IServiceProvider _serviceProvider;
		private readonly TopicOverseerOptions _topicOverseerOptions;
		private double _loop_delay; 

		public TopicOverseer(IServiceProvider serviceProvider, TopicOverseerOptions options)
		{
			_serviceProvider = serviceProvider;
			_topicOverseerOptions = options;
			_loop_delay = _topicOverseerOptions.CAPABILITYSERVICE_FEATURES_TOPIC_METRICS_EVERY_X_SECONDS != null ? Double.Parse(_topicOverseerOptions.CAPABILITYSERVICE_FEATURES_TOPIC_METRICS_EVERY_X_SECONDS) : 60 * 4;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			Console.WriteLine("Starting TopicOverseer");

			var topicsEquality = Prometheus.Metrics.CreateGauge("features_topics_equality",
				"If there are more topics in CC than capsvc, then it'll go in minus. If there are less topics in CC than capsvc, then it'll go in surplus. If everything is fine, then it'll be zero.");
			topicsEquality.Set(0);

			while (!stoppingToken.IsCancellationRequested)
			{
				try
				{
					using (var scope = _serviceProvider.CreateScope())
					{
						var topicDomainService = scope.ServiceProvider.GetRequiredService<ITopicDomainService>();
						var kafkaJanitorRestClient = scope.ServiceProvider.GetRequiredService<IRestClient>();
						
						var capSvcTopics = await topicDomainService.GetAllTopics();
						var connectedTopics = await kafkaJanitorRestClient.Topics.GetAllAsync();
						topicsEquality.Set(capSvcTopics.Count() - connectedTopics.Count());
					}
				}
				catch (Exception err)
				{
					Log.Error(err.Message);
				}
				await Task.Delay(TimeSpan.FromSeconds(_loop_delay), stoppingToken);
			}
		}
	}
}
