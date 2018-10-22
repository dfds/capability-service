using System;
using System.Threading.Tasks;
using DFDS.TeamService.WebApi.Controllers;
using DFDS.TeamService.WebApi.Features.AwsConsoleLogin;
using DFDS.TeamService.WebApi.Features.AwsRoles;
using Moq;
using Xunit;

namespace DFDS.TeamService.Tests
{
    public class TestAwsConsoleUrlController
    {
        [Fact]
        public async Task get_console_link_returns_url()
        {
            var teamIdToAwsConsoleUrlBuilder = new Moq.Mock<ITeamIdToAwsConsoleUrl>();

            teamIdToAwsConsoleUrlBuilder
                .Setup(c => c.CreateUrlAsync(
                        It.IsAny<string>(),
                        It.IsAny<string>()
                    )
                )
                .ReturnsAsync(new Uri("http://bogus"));

            var sut = new AwsConsoleUrlController(teamIdToAwsConsoleUrlBuilder.Object);
            var tokenId = "myFancyToken";
            var teamName = "aGreatTeam";
            var result = await sut.GetConsoleUrl(tokenId, teamName);

            Assert.NotNull(result.Value.AbsoluteUrl);
        }
    }
}