using System;
using System.Threading.Tasks;
using DFDS.CapabilityService.Tests.Builders;
using DFDS.CapabilityService.Tests.TestDoubles;
using DFDS.CapabilityService.WebApi.Application;
using DFDS.CapabilityService.WebApi.Domain.Repositories;
using DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Models;
using DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Repositories;
using DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Services;
using DFDS.CapabilityService.WebApi.Features.Kafka.Infrastructure.RestClients;
using DFDS.CapabilityService.WebApi.Infrastructure.Api;
using DFDS.CapabilityService.WebApi.Infrastructure.Api.DTOs;
using KafkaJanitor.RestClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;
using Capability = DFDS.CapabilityService.WebApi.Domain.Models.Capability;
using ITopicRepository = DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Repositories.ITopicRepository;

namespace DFDS.CapabilityService.Tests.Scenarios
{
	public class TopicWithTooManyPartitions
	{
		private IServiceProvider _serviceProvider;
		private Capability _capability;
		private TopicController _topicController;
		private IActionResult addTopicToCapabilityActionResult;
		private Cluster _cluster;


		[Fact]
		public async Task TopicWithTooManyPartitionsRecipe()
		{
			      Given_a_service_collection_with_a_imMemoryDb();
			await And_a_capability();
			await And_a_cluster();
			await When_a_topic_with_too_many_partitions_is_added();
				  Then_Unprocessable_status_obj_is_returned();
		}

		private void Given_a_service_collection_with_a_imMemoryDb()
		{
			var serviceProviderBuilder = new ServiceProviderBuilder();

			_serviceProvider = serviceProviderBuilder
				.WithServicesFromStartup()
				.WithInMemoryDb()
				.OverwriteService(typeof(IRestClient), new StubKafkaRestClient())
				.Build();
		}

		private async Task And_a_capability()
		{
			var _capabilityApplicationService = _serviceProvider.GetService<ICapabilityApplicationService>();
			_capability = await _capabilityApplicationService.CreateCapability(
				"Foo",
				"This is a capability"
			);
		}
		
		private async Task And_a_cluster()
		{
			var clusterRepository = _serviceProvider.GetService<IClusterRepository>();
			_cluster = await clusterRepository.AddAsync("Dummy test cluster #1", "lkc-9999", true, Guid.Empty);
		}

		private async Task When_a_topic_with_too_many_partitions_is_added()
		{
			_topicController = new TopicController(
				_serviceProvider.GetService<ITopicDomainService>(),
				_serviceProvider.GetService<ITopicRepository>(),
				_serviceProvider.GetService<ICapabilityRepository>(),
				_serviceProvider.GetService<IKafkaJanitorRestClient>(),
				NullLogger<TopicController>.Instance
			);


			addTopicToCapabilityActionResult = await _topicController.AddTopicToCapability(
				_capability.Id.ToString(),
				new TopicInput
				{
					Name = "the topic of the future",
					Description = "The way topics should be",
					Partitions = Int32.MaxValue,
					KafkaClusterId = _cluster.Id.ToString()
				});
		}

		private void Then_Unprocessable_status_obj_is_returned()
		{
			var result =
				(Microsoft.AspNetCore.Mvc.UnprocessableEntityObjectResult)addTopicToCapabilityActionResult;
		}
	}
}
