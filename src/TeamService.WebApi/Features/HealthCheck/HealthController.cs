using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace DFDS.TeamService.WebApi.Controllers
{
    [Route("system")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        private readonly IEnumerable<IExternalDependent> _externalDependentServices;

        public HealthController(IEnumerable<IExternalDependent> externalDependentServices)
        {
            _externalDependentServices = externalDependentServices;
        }

        [HttpGet("health")]
        [ProducesResponseType(200, Type = typeof(string))]
        [ProducesResponseType(504, Type = typeof(string))]
        public async Task<ActionResult<string>> Get(bool deep)
        {
            const string allISWell="Cognito WebApi says this is fine";
            
            if (deep == false)
            {
                return allISWell;
            }

            var unHealthyServiceStatuses = _externalDependentServices
                .Select(dependent => dependent.GetStatusAsync().Result)
                .Where(status => status.IsOk == false);
                

            if (unHealthyServiceStatuses.Any() == false)
            {
                return allISWell;
            }

            var message = "The following dependencies has problems:" + Environment.NewLine +
                          string.Join(
                              Environment.NewLine,
                              unHealthyServiceStatuses.Select(s => s.Message)
                          );
            
            
            return StatusCode(504, message);
        }
    }
}