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
	[Route("api/v1/capabilities")]
    public class TopicController : ControllerBase
    {
        private readonly ITopicRepository _topicRepository;
        private readonly ICapabilityApplicationService _capabilityApplicationService;

        public TopicController(
	        ITopicRepository topicRepository, 
	        ICapabilityApplicationService capabilityApplicationService
	    )
        {
            _topicRepository = topicRepository;
            _capabilityApplicationService = capabilityApplicationService;
        }

        [HttpGet("{id}/topics")]
        public async Task<IActionResult> GetAll(string id)
        {
            var topics = await _topicRepository.GetAll();
            
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
	        try
	        {
		        await _capabilityApplicationService.AddTopic(
			        capabilityId, 
			        input.Name, 
			        input.Description,
			        true
			    );
	        }
	        catch (Exception exception)
	        {
		        return ExceptionToStatusCode(exception);
	        }


	        return Ok();
        }
        
        public ActionResult ExceptionToStatusCode(Exception exception)
        {
	        switch (exception)
	        {
		        case CapabilityDoesNotExistException _:
		        case NotMemberOfCapabilityException _:
			        return NotFound(new {exception.Message});
		        case CapabilityValidationException _:
			        return BadRequest(new {exception.Message});
		        case CapabilityWithSameNameExistException _:
		        case TopicAlreadyExistException _:
			        return Conflict(new {exception.Message});
		        default:
			        throw exception;
	        }
        }
    }
}
