using System;
using System.Threading.Tasks;
using DFDS.CapabilityService.Tests.Builders;
using DFDS.CapabilityService.WebApi.Application;
using DFDS.CapabilityService.WebApi.Domain.Repositories;
using DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Services;
using DFDS.CapabilityService.WebApi.Infrastructure.Api;
using DFDS.CapabilityService.WebApi.Infrastructure.Api.DTOs;
using KafkaJanitor.RestClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Capability = DFDS.CapabilityService.WebApi.Domain.Models.Capability;
using ITopicRepository = DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Repositories.ITopicRepository;

namespace DFDS.CapabilityService.Tests.Scenarios
{
	public class TwoTopicsWithTheSameName
	{
		private IServiceProvider _serviceProvider;
		private Capability _capability;
		private TopicController _topicController;
		private IActionResult _secondTopicAddResponse;

		[Fact]
		public async Task TwoTopicsWithTheSameNameRecipe()
		{
				  Given_a_service_collection_with_a_imMemoryDb();
			await And_a_capability();
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

		private async Task And_a_topic()
		{
			_topicController = new TopicController(
				_serviceProvider.GetService<ITopicDomainService>(),
				_serviceProvider.GetService<ITopicRepository>(),
				_serviceProvider.GetService<ICapabilityRepository>(),
				_serviceProvider.GetService<IServiceAccountService>(),
				_serviceProvider.GetService<IRestClient>()
			);


			await _topicController.AddTopicToCapability(
				_capability.Id.ToString(),
				new TopicInput {Name = "the topic of the future", Description = "The way topics should be"});
		}

		private async Task When_a_topic_is_added()
		{
			_topicController = new TopicController(
				_serviceProvider.GetService<ITopicDomainService>(),
				_serviceProvider.GetService<ITopicRepository>(),
				_serviceProvider.GetService<ICapabilityRepository>(),
				_serviceProvider.GetService<IServiceAccountService>(),
				_serviceProvider.GetService<IRestClient>()
			);


			_secondTopicAddResponse = await _topicController.AddTopicToCapability(
				_capability.Id.ToString(),
				new TopicInput {Name = "the topic of the future", Description = "The way topics should be"});
		}
		
		private void Then_a_conflict_response_is_returned()
		{
			Assert.IsType<ConflictObjectResult>(_secondTopicAddResponse);
		}

	}
}
