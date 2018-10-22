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
                        It.IsAny<Guid>(),
                        It.IsAny<string>()
                    )
                )
                .ReturnsAsync(new Uri("http://bogus"));

            var sut = new AwsConsoleUrlController(teamIdToAwsConsoleUrlBuilder.Object);
            var tokenId = "myFancyToken";
            var teamId = Guid.NewGuid();
            var result = await sut.GetConsoleUrl(teamId ,tokenId);

            Assert.NotNull(result.Value.AbsoluteUrl);
        }
    }
}