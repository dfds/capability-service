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

		public TopicOverseer(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			Console.WriteLine("Starting TopicOverseer");

			var topicsEquality = Prometheus.Metrics.CreateGauge("features_topics_equality",
				"If there are more topics in CC than capsvc, then it'll go in minus. If there are less topics in CC than capsvc, then it'll go in surplus. If everythin is fine, then it'll be zero.");
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
				await Task.Delay(TimeSpan.FromMinutes(4), stoppingToken);
			}
		}
	}
}
