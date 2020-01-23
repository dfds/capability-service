using System;
using System.Linq;
using System.Runtime.CompilerServices;
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
    [Route("api/v1/adsync")]
    public class ADSyncController : ControllerBase
    {
        private readonly ICapabilityApplicationService _capabilityApplicationService;

        public ADSyncController(ICapabilityApplicationService capabilityApplicationService)
        {
            _capabilityApplicationService = capabilityApplicationService;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetAllCapabilities()
        {
            var capabilities = await _capabilityApplicationService.GetAllCapabilities();
            var v1Capabilities = capabilities.Where(c => string.IsNullOrEmpty(c.RootId));
            var v2CapabilitiesFiltered = capabilities
                .Where(c => !string.IsNullOrEmpty(c.RootId))
                .Where(c => c.Contexts != null && c.Contexts.Any())
                .Where(c => c.Contexts.Any(ctx=>!string.IsNullOrEmpty(ctx.AWSAccountId) && !string.IsNullOrEmpty(ctx.AWSRoleArn) && !string.IsNullOrEmpty(ctx.AWSRoleEmail)));

            var dtos = v1Capabilities.Concat(v2CapabilitiesFiltered).Select(CapabilityADSync.Create);
            
            return Ok(new CapabilityADSyncResponse()
            {
                Items = dtos.ToArray()
            });
        }

    }
}
