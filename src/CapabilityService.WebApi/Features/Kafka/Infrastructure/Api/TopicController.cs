using System;
using System.Linq;
using System.Threading.Tasks;
using DFDS.CapabilityService.WebApi.Domain.Models;
using DFDS.CapabilityService.WebApi.Domain.Repositories;
using DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Services;
using DFDS.CapabilityService.WebApi.Infrastructure.Api.DTOs;
using KafkaJanitor.RestClient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Topic = DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Models.Topic;
using ITopicRepository = DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Repositories.ITopicRepository;
using MessageContract = DFDS.CapabilityService.WebApi.Domain.Models.MessageContract;

namespace DFDS.CapabilityService.WebApi.Infrastructure.Api
{
	[Authorize(AuthenticationSchemes = "AzureADBearer")]
	[ApiController]
	[Route("api/v1/capabilities")]
	public class TopicController : ControllerBase
	{
		private readonly ITopicDomainService _topicDomainService;
		private readonly ITopicRepository _topicRepository;
		private readonly ICapabilityRepository _capabilityRepository;
		private readonly IServiceAccountService _serviceAccountService;
		private readonly IRestClient _kafkaJanitorClient;

		public TopicController(
			ITopicDomainService topicDomainService,
			ITopicRepository topicRepository,
			ICapabilityRepository capabilityRepository,
			IServiceAccountService serviceAccountService,
			IRestClient kafkaJanitorClient
		)
		{
			_topicDomainService = topicDomainService;
			_topicRepository = topicRepository;
			_capabilityRepository = capabilityRepository;
			_serviceAccountService = serviceAccountService;
			_kafkaJanitorClient = kafkaJanitorClient;
		}

		[HttpGet("{id}/topics")]
		public async Task<IActionResult> GetAll(string id)
		{
			var topics = await _topicRepository.GetAllAsync();

			var capabilityId = Guid.Empty;
			Guid.TryParse(id, out capabilityId);

			var result = new
			{
				Items = topics
					.Where(t => t.CapabilityId == capabilityId)
					.Select(DTOs.Topic.CreateFrom)
					.ToArray()
			};

			return Ok(result);
		}


		[HttpPost("{id}/topics")]
		public async Task<IActionResult> AddTopicToCapability(string id, [FromBody] TopicInput input)
		{
			var capabilityId = Guid.Empty;
			Guid.TryParse(id, out capabilityId);

			if (capabilityId == Guid.Empty) return BadRequest(new {Message = $"the capability id: {id} is malformed"});

			var capability = await
				_capabilityRepository.Get(capabilityId);
			var capabilityName = new CapabilityName(capability.Name);

			try
			{
				var topic = Topic.Create(
					capabilityId,
					capabilityName,
					input.Name,
					input.Description,
					input.Partitions
				);


				await _topicDomainService.CreateTopic(
					topic,
					input.DryRun
				);

				if (!input.DryRun)
				{
					await _kafkaJanitorClient.Topics.CreateAsync(new KafkaJanitor.RestClient.Features.Topics.Models.Topic
					{
						Name = topic.Name.Name,
						Description = topic.Description,
						Partitions = topic.Partitions
					});
					await _serviceAccountService.EnsureServiceAccountAvailability(capability, new Domain.Models.Topic(topic.Id.Id, topic.Name.Name, topic.Description, false, topic.CapabilityId, new MessageContract[]{}));
				}

				var topicDto = DTOs.Topic.CreateFrom(topic);
				return Ok(topicDto);

			}
			catch (Exception exception)
			{
				var actionResult = ExceptionToStatusCode.Convert(exception);
				if (actionResult == null) throw;
				return actionResult;
			}


		}
	}
}
