using System;
using System.Linq;
using System.Threading.Tasks;
using DFDS.CapabilityService.WebApi.Application;
using DFDS.CapabilityService.WebApi.Domain.Exceptions;
using DFDS.CapabilityService.WebApi.Domain.Repositories;
using DFDS.CapabilityService.WebApi.Infrastructure.Api.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DFDS.CapabilityService.WebApi.Infrastructure.Api
{
	[Authorize(AuthenticationSchemes = "AzureADBearer")]
	[ApiController]
    [Route("api/v1/topics")]
    public class TopicController : ControllerBase
    {
        private readonly ITopicRepository _topicRepository;
        private readonly ITopicApplicationService _topicApplicationService;

        public TopicController(ITopicRepository topicRepository, ITopicApplicationService topicApplicationService)
        {
            _topicRepository = topicRepository;
            _topicApplicationService = topicApplicationService;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetAll()
        {
            var topics = await _topicRepository.GetAll();

            var result = new
            {
                Items = topics
                    .Select(DTOs.Topic.CreateFrom)
                    .ToArray()
            };

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSingle(string id)
        {
            Guid topicId;
            Guid.TryParse(id, out topicId);

            var topic = await _topicRepository.Get(topicId);

            if (topic == null)
            {
                return NotFound();
            }

            var result = DTOs.Topic.CreateFrom(topic);

            return Ok(result);
        }

        [HttpGet("{id}/messagecontracts")]
        public async Task<IActionResult> GetAllMessageContracts(string id)
        {
            Guid topicId;
            Guid.TryParse(id, out topicId);

            var topic = await _topicRepository.Get(topicId);

            if (topic == null)
            {
                return NotFound();
            }

            var result = new
            {
                Items = topic.MessageContracts
                    .Select(MessageContract.CreateFrom)
                    .ToArray()
            };

            return Ok(result);
        }

        [HttpDelete("{id}/messagecontracts/{messageType}")]
        public async Task<IActionResult> RemoveMessageContract([FromRoute]string id, [FromRoute]string messageType)
        {
            Guid topicId;
            Guid.TryParse(id, out topicId);

            try
            {
                await _topicApplicationService.RemoveMessageContract(
                    topicId: topicId,
                    type: messageType
                );

                return NoContent();
            }
            catch (TopicDoesNotExistException)
            {
                return NotFound();
            }
        }

        [HttpPut("{id}/messagecontracts/{messageType}")]
        public async Task<IActionResult> AddOrUpdateMessageContract([FromRoute]string id, [FromRoute]string messageType, [FromBody]MessageContractInput messageContractInput)
        {
            Guid topicId;
            Guid.TryParse(id, out topicId);

            try
            {
                await _topicApplicationService.UpdateMessageContract(
                    topicId: topicId,
                    type: messageType,
                    description: messageContractInput.Description,
                    content: messageContractInput.Content
                );

                return NoContent();
            }
            catch (TopicDoesNotExistException)
            {
                return NotFound();
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTopicDetails([FromRoute]string id, [FromBody]TopicInput input)
        {
            Guid topicId;
            Guid.TryParse(id, out topicId);

            try
            {
                await _topicApplicationService.UpdateTopic(
                    topicId: topicId,
                    name: input.Name,
                    description: input.Description,
                    isPrivate: input.IsPrivate
                );

                return NoContent();
            }
            catch (TopicDoesNotExistException)
            {
                return NotFound();
            }
            catch (TopicAlreadyExistException)
            {
                return BadRequest(new
                {
                    Message = $"A topic with the name \"{input.Name}\" already exist."
                });
            }
        }
    }
}
