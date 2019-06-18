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

            return Ok(new CapabilityADSyncResponse()
            {
                Items = capabilities
                        .Select(CapabilityADSync.Create)
                        .ToArray()
            });
        }

    }
}