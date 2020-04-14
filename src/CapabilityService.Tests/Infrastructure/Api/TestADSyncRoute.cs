using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using DFDS.CapabilityService.Tests.Builders;
using DFDS.CapabilityService.Tests.TestDoubles;
using DFDS.CapabilityService.WebApi.Application;
using DFDS.CapabilityService.WebApi.Domain.Exceptions;
using DFDS.CapabilityService.WebApi.Domain.Models;
using Xunit;
using DFDS.CapabilityService.Tests.Infrastructure.Authentication;
namespace DFDS.CapabilityService.Tests.Infrastructure.Api
{
    public class TestADSyncRoute
    {
        [Fact]
        public async Task get_all_capabilities_returns_expected_status_code()
        {

            using (var builder = new HttpClientBuilder())
            {
                var client = builder
                    .WithService<ICapabilityApplicationService>(new StubCapabilityApplicationService())
                    .Build();

                var request = new HttpRequestMessage(HttpMethod.Get, "api/v1/adsync");
                request.Headers.Authorization = BasicAuthCredentials.BASIC_AUTHENTICATION_HEADER_VALUE;
                var response = await client.SendAsync(request);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }
        
        [Fact]
        public async Task get_all_capabilities_returns_expected_body_when_no_capabilities_available()
        {
            using (var builder = new HttpClientBuilder())
            {
                var client = builder
                    .WithService<ICapabilityApplicationService>(new StubCapabilityApplicationService())
                    .Build();

                var request = new HttpRequestMessage(HttpMethod.Get, "api/v1/adsync");
                request.Headers.Authorization = BasicAuthCredentials.BASIC_AUTHENTICATION_HEADER_VALUE;
                var response = await client.SendAsync(request);

                Assert.Equal(
                    expected: "{\"items\":[]}",
                    actual: await response.Content.ReadAsStringAsync()
                );
            }
        }

        [Fact]
        public async Task get_all_capabilities_returns_expected_list_with_only_v1()
        {
            using (var builder = new HttpClientBuilder())
            {
                var capOld = new CapabilityBuilder().WithRootId(null).Build();

                Capability[] stubCapabilities = new[] {capOld};
                var client = builder
                    .WithService<ICapabilityApplicationService>(new StubCapabilityApplicationService(stubCapabilities: stubCapabilities))
                    .Build();

                var request = new HttpRequestMessage(HttpMethod.Get, "api/v1/adsync");
                request.Headers.Authorization = BasicAuthCredentials.BASIC_AUTHENTICATION_HEADER_VALUE;
                var response = await client.SendAsync(request);
                var content = await response.Content.ReadAsStringAsync();
                
                Assert.Equal(
                    expected: $"{{\"items\":[{{\"identifier\":\"Foo\",\"members\":[],\"isV1\":true,\"awsAccountId\":null,\"awsRoleArn\":null}}]}}",
                    actual: content);
            }
        }

        [Fact]
        public async Task get_all_capabilities_return_expected_list_with_only_v2()
        {
            using (var builder = new HttpClientBuilder())
            {
                var capNewWithoutContext = new CapabilityBuilder().Build();
                var ctx = new ContextBuilder().WithAccountId(null).WithRoleArn(null).WithRoleEmail(null).Build();
                var capNewWithContext = new CapabilityBuilder().WithContexts(ctx).Build();
                var populatedContext = new ContextBuilder().Build();
                var capNewWithPopulatedContext = new CapabilityBuilder().WithContexts(populatedContext).Build();

                Capability[] stubCapabilities = new[] {capNewWithContext, capNewWithoutContext, capNewWithPopulatedContext};
                var client = builder
                    .WithService<ICapabilityApplicationService>(new StubCapabilityApplicationService(stubCapabilities: stubCapabilities))
                    .Build();

                var request = new HttpRequestMessage(HttpMethod.Get, "api/v1/adsync");
                request.Headers.Authorization = BasicAuthCredentials.BASIC_AUTHENTICATION_HEADER_VALUE;
                var response = await client.SendAsync(request);
                var content = await response.Content.ReadAsStringAsync();
                
                Assert.Equal(
                    expected: $"{{\"items\":[{{\"identifier\":\"foo-582a4\",\"members\":[],\"isV1\":false,\"awsAccountId\":\"222222222222\",\"awsRoleArn\":\"arn:aws:iam::528563840976:role/aws-elasticbeanstalk-ec2-role\"}}]}}",
                    actual: content);
            }
        }
        
        [Fact]
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

                Capability[] stubCapabilities = new[] {capOld, capNewWithContext, capNewWithoutContext, capNewWithPopulatedContext};
                var client = builder
                    .WithService<ICapabilityApplicationService>(new StubCapabilityApplicationService(stubCapabilities: stubCapabilities))
                    .Build();

                var request = new HttpRequestMessage(HttpMethod.Get, "api/v1/adsync");
                request.Headers.Authorization = BasicAuthCredentials.BASIC_AUTHENTICATION_HEADER_VALUE;
                var response = await client.SendAsync(request);
                var content = await response.Content.ReadAsStringAsync();

                Assert.Equal(
                    expected: $"{{\"items\":[{{\"identifier\":\"Foo\",\"members\":[],\"isV1\":true,\"awsAccountId\":null,\"awsRoleArn\":null}},{{\"identifier\":\"foo-582a4\",\"members\":[],\"isV1\":false,\"awsAccountId\":\"{populatedContext.AWSAccountId}\",\"awsRoleArn\":\"{populatedContext.AWSRoleArn}\"}}]}}",
                    actual: content
                );
            }
        }
    }
}
