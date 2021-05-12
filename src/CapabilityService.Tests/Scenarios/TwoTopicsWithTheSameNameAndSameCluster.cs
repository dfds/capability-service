using System;
using System.Threading;
using System.Threading.Tasks;
using DFDS.CapabilityService.Tests.Builders;
using DFDS.CapabilityService.Tests.TestDoubles;
using DFDS.CapabilityService.WebApi.Application;
using DFDS.CapabilityService.WebApi.Domain.Repositories;
using DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Repositories;
using DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Services;
using DFDS.CapabilityService.WebApi.Features.Kafka.Infrastructure.Persistence.EntityFramework.DAOs;
using DFDS.CapabilityService.WebApi.Features.Kafka.Infrastructure.RestClients;
using DFDS.CapabilityService.WebApi.Infrastructure.Api;
using DFDS.CapabilityService.WebApi.Infrastructure.Api.DTOs;
using KafkaJanitor.RestClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Capability = DFDS.CapabilityService.WebApi.Domain.Models.Capability;
using Cluster = DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Models.Cluster;
using ITopicRepository = DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Repositories.ITopicRepository;
using Topic = DFDS.CapabilityService.WebApi.Infrastructure.Api.DTOs.Topic;

namespace DFDS.CapabilityService.Tests.Scenarios
{
	public class TwoTopicsWithTheSameNameAndSameCluster
	{
		private IServiceProvider _serviceProvider;
		private Capability _capability;
		private TopicController _topicController;
		private IActionResult _secondTopicAddResponse;
		private Cluster _cluster;

		[Fact]
		public async Task TwoTopicsWithTheSameNameAndSameClusterRecipe()
		{
				  Given_a_service_collection_with_a_imMemoryDb();
			await And_a_capability();
			await And_a_cluster();
			await And_a_topic();
			await When_a_topic_is_added();
				  Then_a_conflict_response_is_returned();
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

		private async Task And_a_topic()
		{
			_topicController = new TopicController(
				_serviceProvider.GetService<ITopicDomainService>(),
				_serviceProvider.GetService<ITopicRepository>(),
				_serviceProvider.GetService<ICapabilityRepository>(),
				_serviceProvider.GetService<IKafkaJanitorRestClient>()
			);


			var result = await _topicController.AddTopicToCapability(
				_capability.Id.ToString(),
				new TopicInput {Name = "the topic of the future", Description = "The way topics should be", Partitions = 2, KafkaClusterId = _cluster.Id.ToString(), DryRun = false});
			
			// Creation of a Topic as it is now needs to be refactored to avoid having to put a random delay/sleep here.
			Thread.Sleep(2000);
				var removeThisAllTopics = await _topicController.GetAll();
				var actionResult = await _topicController
					.GetAllByCapability(_capability.Id.ToString());

				var okResult = actionResult as OkObjectResult;

				var topicsCollection = (Topic[])okResult.Value
					.GetType()
					.GetProperty("Items")
					.GetValue(okResult.Value);

				var topics = topicsCollection.Any() ? topicsCollection : null;


			Assert.Single(topics);
		}
		
		private async Task When_a_topic_is_added()
			{
			_topicController = new TopicController(
				_serviceProvider.GetService<ITopicDomainService>(),
				_serviceProvider.GetService<ITopicRepository>(),
				_serviceProvider.GetService<ICapabilityRepository>(),
				_serviceProvider.GetService<IKafkaJanitorRestClient>()
			);


			_secondTopicAddResponse = await _topicController.AddTopicToCapability(
				_capability.Id.ToString(),
				new TopicInput {Name = "the topic of the future", Description = "The way topics should be", Partitions = 2, KafkaClusterId = _cluster.Id.ToString(), DryRun = false});
		}
		
		private void Then_a_conflict_response_is_returned()
		{
			Assert.IsType<ConflictObjectResult>(_secondTopicAddResponse);
		}
		
		public async Task<TResult> DoUntilResultOr5Sec<TResult>(Func<Task<TResult>> function)
		{
			var endTime = DateTime.Now.AddSeconds(5);
			dynamic result = null;
			do
			{
				result = await function();
				if (result == null)
				{
					Thread.Sleep(1000);
				}
			} while (result == null && DateTime.Now < endTime);

			return result;
		}

	}
}
