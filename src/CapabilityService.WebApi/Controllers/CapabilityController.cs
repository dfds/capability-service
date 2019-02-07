using System;
using System.Linq;
using System.Threading.Tasks;
using DFDS.CapabilityService.WebApi.Models;
using DFDS.CapabilityService.WebApi.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace DFDS.CapabilityService.WebApi.Controllers
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
                        .Select(DtoHelper.ConvertToDto)
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

            var dto = DtoHelper.ConvertToDto(capability);

            return Ok(dto);
        }

        [HttpPost("")]
        public async Task<IActionResult> CreateCapability(CapabilityInput input)
        {
            try {
                var capability = await _capabilityApplicationService.CreateCapability(input.Name);
                var dto = DtoHelper.ConvertToDto(capability);

                return CreatedAtAction(
                    actionName: nameof(GetCapability),
                    routeValues: new {id = capability.Id},
                    value: dto
                );
            } catch (CapabilityValidationException tve) {
                return BadRequest(new
                {
                    Message = tve.Message
                });
            }
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
    }

    public class MemberInput
    {
        public string Email { get; set; }
    }
}