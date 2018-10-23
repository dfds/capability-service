using System.Net;
using System.Threading.Tasks;
using DFDS.TeamService.Tests.Builders;
using DFDS.TeamService.WebApi.Features.AwsConsoleLogin;
using Moq;
using Shouldly;
using Xunit;

namespace DFDS.TeamService.Tests.Features.AwsConsoleUrl
{
    public class AwsConsoleUrlControllerRoutesFacts
    {
        private const string CONSOLE_URL_TEMPLATE = "api/teams/{id}/aws/console-url";
    
        
        [Fact]
        public async Task GetConsoleUrl_NOT_GIVEN_Guid_EXPECT_BadRequest()
        {
            using (var builder = new HttpClientBuilder())
            {
                // Arrange
                var teamIdToAwsConsoleUrl = new Mock<ITeamIdToAwsConsoleUrl>();
                var client = builder
                    .WithService(teamIdToAwsConsoleUrl.Object)
                    .Build();

                
                // Act
                var response = await client.GetAsync(CONSOLE_URL_TEMPLATE.Replace("{id}", "Not_a_Guid") + "?idtoken=a_id_token");

                
                // Assert
                response
                    .StatusCode
                    .ShouldBe(HttpStatusCode.BadRequest);
            }
        }
    }
}