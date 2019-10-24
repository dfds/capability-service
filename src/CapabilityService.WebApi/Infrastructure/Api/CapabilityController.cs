using System;
using System.Linq;
using System.Threading.Tasks;
using DFDS.CapabilityService.WebApi.Application;
using DFDS.CapabilityService.WebApi.Domain.Exceptions;
using DFDS.CapabilityService.WebApi.Infrastructure.Api.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace DFDS.CapabilityService.WebApi.Infrastructure.Api
{
    [ApiController]
    [Route("api/v1/capabilities")]
    public class CapabilityController : ControllerBase
    {
        private readonly ICapabilityApplicationService _capabilityApplicationService;

        public CapabilityController(ICapabilityApplicationService capabilityApplicationService)
        {
            _capabilityApplicationService = capabilityApplicationService;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetAllCapabilities()
        {
            var capabilities = await _capabilityApplicationService.GetAllCapabilities();

            return Ok(new CapabilityResponse
            {
                Items = capabilities
                        .Select(Capability.Create)
                        .ToArray()
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCapability(string id)
        {
            var capabilityId = Guid.Empty;
            Guid.TryParse(id, out capabilityId);

            var capability = await _capabilityApplicationService.GetCapability(capabilityId);

            if (capability == null)
            {
                return NotFound(new
                {
                    Message = $"A capability with id \"{id}\" could not be found."
                });
            }

            var topics = await _capabilityApplicationService.GetTopicsForCapability(capabilityId);
            var dto = CapabilityDetails.Create(capability, topics);

            return Ok(dto);
        }

        [HttpPost("")]
        public async Task<IActionResult> CreateCapability(CapabilityInput input)
        {
            Domain.Models.Capability capability;
            try
            {
                capability = await _capabilityApplicationService.CreateCapability(input.Name, input.Description);
            }
            catch (CapabilityValidationException tve)
            {
                return BadRequest(new
                {
                    Message = tve.Message
                });
            }
            catch (CapabilityWithSameNameExistException)
            {
                return Conflict(new
                {
                    Message = $"A capability with the name:'{input.Name}' already exits, please give your capability a other name."
                });
            }
            
            var dto = Capability.Create(capability);

            return CreatedAtAction(
                actionName: nameof(GetCapability),
                routeValues: new {id = capability.Id},
                value: dto
            );
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCapability(string id, [FromBody] CapabilityInput input)
        {
            var capabilityId = Guid.Empty;
            Guid.TryParse(id, out capabilityId);

            try
            {
                var capability = await _capabilityApplicationService.UpdateCapability(capabilityId, input.Name, input.Description);
                var dto = Capability.Create(capability);

                return Ok(
                    value: dto
                );
            } catch (CapabilityDoesNotExistException) {
                return NotFound(new
                {
                    Message = $"Capability with id {id} could not be found."
                });                
            } catch (CapabilityValidationException tve) {
                return BadRequest(new
                {
                    Message = tve.Message
                });
            }
        }
        
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCapability(string id)
        {
            var capabilityId = Guid.Empty;
            Guid.TryParse(id, out capabilityId);

            var capability = await _capabilityApplicationService.GetCapability(capabilityId);

            if (capability == null)
            {
                return NotFound(new
                {
                    Message = $"A capability with id \"{id}\" could not be found."
                });
            }


            await _capabilityApplicationService.DeleteCapability(capabilityId);
            return Ok();
        }
        
        [HttpPost("{id}/members")]
        public async Task<IActionResult> AddMemberToCapability(string id, [FromBody] MemberInput input)
        {
            var capabilityId = Guid.Empty;
            Guid.TryParse(id, out capabilityId);

            try
            {
                await _capabilityApplicationService.JoinCapability(capabilityId, input.Email);
            }
            catch (CapabilityDoesNotExistException)
            {
                return NotFound(new
                {
                    Message = $"Capability with id {id} could not be found."
                });
            }

            return Ok();
        }

        [HttpDelete("{id}/members/{memberEmail}")]
        public async Task<IActionResult> RemoveMemberFromCapability([FromRoute] string id, [FromRoute] string memberEmail)
        {
            var capabilityId = Guid.Empty;
            Guid.TryParse(id, out capabilityId);

            try
            {
                await _capabilityApplicationService.LeaveCapability(capabilityId, memberEmail);
            }
            catch (CapabilityDoesNotExistException)
            {
                return NotFound(new
                {
                    Message = $"Capability with id {id} could not be found."
                });
            }
            catch (NotMemberOfCapabilityException)
            {
                return NotFound(new
                {
                    Message = $"Capability with id {id} does not have member \"{memberEmail}\"."
                });
            }

            return Ok();
        }
        
        [HttpPost("{id}/contexts")]
        public async Task<IActionResult> AddContextToCapability(string id, [FromBody] ContextInput input)
        {
            var capabilityId = Guid.Empty;
            Guid.TryParse(id, out capabilityId);

            try
            {
                await _capabilityApplicationService.AddContext(capabilityId, input.Name);
            }
            catch (CapabilityDoesNotExistException)
            {
                return NotFound(new
                {
                    Message = $"Capability with id {id} could not be found."
                });
            }

            return Ok();
        }

        [HttpPost("{id}/topics")]
        public async Task<IActionResult> AddTopicToCapability(string id, [FromBody] TopicInput input)
        {
            var capabilityId = Guid.Empty;
            Guid.TryParse(id, out capabilityId);

            try
            {
                await _capabilityApplicationService.AddTopic(capabilityId, input.Name, input.Description, input.IsPrivate);
            }
            catch (CapabilityDoesNotExistException)
            {
                return NotFound(new
                {
                    Message = $"Capability with id {id} could not be found."
                });
            }
            catch (TopicAlreadyExistException)
            {
                return Conflict(new
                {
                    Message = $"A topic with name \"{input.Name}\" already exist."
                });
            }

            return Ok();
        }
    }
}