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

        
        public async Task get_all_capabilities_returns_expected_list_with_a_mix_of_v1_and_v2()
        {
            using (var builder = new HttpClientBuilder())
            {
                var capOld = new CapabilityBuilder().WithRootId(null).Build();
                var capNewWithoutContext = new CapabilityBuilder().Build();
                var ctx = new ContextBuilder().WithAccountId(null).WithRoleArn(null).WithRoleEmail(null).Build();
                var capNewWithContext = new CapabilityBuilder().WithContexts(ctx).Build();
                var populatedContext = new ContextBuilder().Build();
                var capNewWithPopulatedContext = new CapabilityBuilder().WithContexts(populatedContext).Build();
                
                var client = builder
                    .WithService<ICapabilityApplicationService>(new StubCapabilityApplicationService(capOld, capNewWithContext, capNewWithoutContext, capNewWithPopulatedContext))
                    .Build();

                var response = await client.GetAsync("api/v1/adsync");
                var content = await response.Content.ReadAsStringAsync();

                Assert.Equal(
                    expected: $"{{\"items\":[{{\"identifier\":\"foo\",\"members\":[],\"isV1\":true,\"awsAccountId\":null,\"awsRoleArn\":null}},{{\"identifier\":\"foo-582a4\",\"members\":[],\"isV1\":false,\"awsAccountId\":\"{populatedContext.AWSAccountId}\",\"awsRoleArn\":\"{populatedContext.AWSRoleArn}\"}}]}}",
                    actual: content
                );
            }
        }
    }
}