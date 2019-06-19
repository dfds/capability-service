using System;
using System.Net;
using System.Threading.Tasks;
using DFDS.CapabilityService.Tests.Builders;
using DFDS.CapabilityService.Tests.TestDoubles;
using DFDS.CapabilityService.WebApi.Application;
using DFDS.CapabilityService.WebApi.Domain.Exceptions;
using DFDS.CapabilityService.WebApi.Domain.Models;
using Xunit;

namespace DFDS.CapabilityService.Tests.Infrastructure.Api
{
    public class TestADSyncRoute
    {
        [Fact(Skip = "Suspect concurrency issues")]
        public async Task get_all_capabilities_returns_expected_status_code()
        {
            using (var builder = new HttpClientBuilder())
            {
                var client = builder
                    .WithService<ICapabilityApplicationService>(new StubCapabilityApplicationService())
                    .Build();

                var response = await client.GetAsync("api/v1/adsync");

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [Fact(Skip="Suspect concurrency issues")]
        public async Task get_all_capabilities_returns_expected_body_when_no_capabilities_available()
        {
            using (var builder = new HttpClientBuilder())
            {
                var client = builder
                    .WithService<ICapabilityApplicationService>(new StubCapabilityApplicationService())
                    .Build();

                var response = await client.GetAsync("api/v1/adsync");

                Assert.Equal(
                    expected: "{\"items\":[]}",
                    actual: await response.Content.ReadAsStringAsync()
                );
            }
        }

      
    }
}