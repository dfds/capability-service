using System;
using System.Linq;
using System.Threading.Tasks;
using DFDS.CapabilityService.WebApi.Application;
using DFDS.CapabilityService.WebApi.Domain.Exceptions;
using DFDS.CapabilityService.WebApi.Infrastructure.Api.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DFDS.CapabilityService.WebApi.Infrastructure.Api
{
	[Authorize(AuthenticationSchemes = "AzureADBearer")]
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

			CapabilityDetails dto;
			try
			{
				var capability = await _capabilityApplicationService.GetCapability(capabilityId);

				var topics = await _capabilityApplicationService.GetTopicsForCapability(capabilityId);
				dto = CapabilityDetails.Create(capability, topics);
			}
			catch (Exception exception)
			{
				return ExceptionToStatusCode(exception);
			}


			return Ok(dto);
		}

		[HttpPost("")]
		public async Task<IActionResult> CreateCapability(CapabilityInput input)
		{
			Domain.Models.Capability capability;
			Capability dto;
			try
			{
				capability = await _capabilityApplicationService.CreateCapability(input.Name, input.Description);

				dto = Capability.Create(capability);
			}
			catch (Exception exception)
			{
				return ExceptionToStatusCode(exception);
			}

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
				var capability =
					await _capabilityApplicationService.UpdateCapability(capabilityId, input.Name, input.Description);
				var dto = Capability.Create(capability);

				return Ok(
					value: dto
				);
			}
			catch (Exception exception)
			{
				return ExceptionToStatusCode(exception);
			}
		}


		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteCapability(string id)
		{
			var capabilityId = Guid.Empty;
			Guid.TryParse(id, out capabilityId);

			try
			{
				await _capabilityApplicationService.DeleteCapability(capabilityId);
			}
			catch (Exception exception)
			{
				return ExceptionToStatusCode(exception);
			}


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
			catch (Exception exception)
			{
				return ExceptionToStatusCode(exception);
			}

			return Ok();
		}

		[HttpDelete("{id}/members/{memberEmail}")]
		public async Task<IActionResult> RemoveMemberFromCapability([FromRoute] string id,
			[FromRoute] string memberEmail)
		{
			var capabilityId = Guid.Empty;
			Guid.TryParse(id, out capabilityId);

			try
			{
				await _capabilityApplicationService.LeaveCapability(capabilityId, memberEmail);
			}
			catch (Exception exception)
			{
				return ExceptionToStatusCode(exception);
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
