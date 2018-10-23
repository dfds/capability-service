using System;
using System.Net;
using System.Threading.Tasks;
using DFDS.TeamService.Tests.Builders;
using DFDS.TeamService.WebApi.Features.AwsConsoleLogin;
using Moq;
using Shouldly;
using Xunit;

namespace DFDS.TeamService.Tests.Features.AwsConsoleUrl
{
    public class AwsConsoleUrlRoutesFacts
    {
        private const string CONSOLE_URL_TEMPLATE = "api/teams/{id}/aws/console-url";


        [Fact]
        public async Task GetConsoleUrl_NOT_GIVEN_Guid_EXPECT_BadRequest()
        {
            using (var builder = new HttpClientBuilder())
            {
                // Arrange
                var teamIdToAwsConsoleUrlBuilder = new Mock<ITeamIdToAwsConsoleUrl>();
                var client = builder
                    .WithService(teamIdToAwsConsoleUrlBuilder.Object)
                    .Build();


                // Act
                var response =
                    await client.GetAsync(CONSOLE_URL_TEMPLATE.Replace("{id}", "Not_a_Guid") + "?idtoken=a_id_token");


                // Assert
                response
                    .StatusCode
                    .ShouldBe(HttpStatusCode.BadRequest);
            }
        }

        [Fact]
        public async Task GetConsoleUrl_NOT_GIVEN_id_token_EXPECT_BadRequest()
        {
            using (var builder = new HttpClientBuilder())
            {
                // Arrange
                var teamIdToAwsConsoleUrlBuilder = new Mock<ITeamIdToAwsConsoleUrl>();
                var client = builder
                    .WithService(teamIdToAwsConsoleUrlBuilder.Object)
                    .Build();


                // Act
                var response = await client.GetAsync(CONSOLE_URL_TEMPLATE.Replace("{id}", Guid.NewGuid().ToString()) );


                // Assert
                response
                    .StatusCode
                    .ShouldBe(HttpStatusCode.BadRequest);
            }
        }
        
        
        [Fact]
        public async Task GetConsoleUrl_GIVEN_Guid_AND_id_token_EXPECT_Ok()
        {
            using (var builder = new HttpClientBuilder())
            {
                // Arrange
                var teamIdToAwsConsoleUrlBuilder = new Mock<ITeamIdToAwsConsoleUrl>();
                teamIdToAwsConsoleUrlBuilder
                    .Setup(t => t.CreateUrlAsync(It.IsAny<Guid>(), It.IsAny<string>()))
                    .ReturnsAsync(new Uri("//happy"));

                var client = builder
                    .WithService(teamIdToAwsConsoleUrlBuilder.Object)
                    .Build();


                // Act
                var response = await client.GetAsync(CONSOLE_URL_TEMPLATE.Replace("{id}", Guid.NewGuid().ToString()) +"?idtoken=a_id_token" );


                // Assert
                response
                    .StatusCode
                    .ShouldBe(HttpStatusCode.OK);
            }
        }
    }
}