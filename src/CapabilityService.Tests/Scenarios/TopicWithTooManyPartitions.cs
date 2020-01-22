using System;
using System.Threading.Tasks;
using DFDS.CapabilityService.Tests.Builders;
using DFDS.CapabilityService.WebApi.Application;
using DFDS.CapabilityService.WebApi.Domain.Repositories;
using DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Services;
using DFDS.CapabilityService.WebApi.Infrastructure.Api;
using DFDS.CapabilityService.WebApi.Infrastructure.Api.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
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

		[Fact]
		public async Task TopicWithTooManyPartitionsRecipe()
		{
			Given_a_service_collection_with_a_imMemoryDb();
			await And_a_capability();
			await When_a_topic_with_too_many_partitions_is_added();
			await Then_Unprocessable_status_obj_is_returned();
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

		private async Task When_a_topic_with_too_many_partitions_is_added()
		{
			_topicController = new TopicController(
				_serviceProvider.GetService<ITopicDomainService>(),
				_serviceProvider.GetService<ITopicRepository>(),
				_serviceProvider.GetService<ICapabilityRepository>()
			);


			addTopicToCapabilityActionResult = await _topicController.AddTopicToCapability(
				_capability.Id.ToString(),
				new TopicInput
				{
					Name = "the topic of the future",
					Description = "The way topics should be",
					Partitions = Int32.MaxValue
				});
		}

		private async Task Then_Unprocessable_status_obj_is_returned()
		{
			var nobodyCares =
				(Microsoft.AspNetCore.Mvc.UnprocessableEntityObjectResult)addTopicToCapabilityActionResult;
		}
	}
}
