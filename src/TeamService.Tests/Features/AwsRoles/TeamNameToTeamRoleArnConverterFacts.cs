using System;
using DFDS.TeamService.WebApi.Features.AwsRoles;
using Xunit;

namespace DFDS.TeamService.Tests.Features.AwsRoles
{
    public class TeamNameToTeamRoleArnConverterFacts
    {
        [Fact]
        public void GIVEN_simple_string_EXPECT_ARN()
        {
            // Arrange
            var teamNameToArnRoleNameConverter = new TeamNameToTeamRoleArnConverter("123456789012");

            var simpleTeamName = "team";
            
            
            // Act
            var arnResult = teamNameToArnRoleNameConverter.Convert(simpleTeamName);
            
            
            // Assert
            var expectedArn = "arn:aws:iam::123456789012:role/team";
            Assert.Equal(expectedArn, arnResult);
        }


        [Fact]
        public void GIVEN_string_WITH_length_of_65_EXPECT_Exception()
        {
            // Arrange
            var teamNameToArnRoleNameConverter = new TeamNameToTeamRoleArnConverter("123456789012");
            
            var longTeamName = (new Guid().ToString() + new Guid() + new Guid()).Substring(0, 65);

            
            // Act
            // Assert
            Assert.Throws<ArgumentException>(() =>
                teamNameToArnRoleNameConverter.Convert(longTeamName)
            );
        }

        
        [Fact]
        public void GIVEN_string_WITH_double_dots_EXPECT_Exception()
        {
            // Arrange
            var teamNameToArnRoleNameConverter = new TeamNameToTeamRoleArnConverter("123456789012");
            
            var teamNameWithDoubleDots = "team..1";

            
            // Act
            // Assert
            Assert.Throws<ArgumentException>(() =>
                teamNameToArnRoleNameConverter.Convert(teamNameWithDoubleDots)
            );
        }

        [Fact]
        public void GIVEN_spaces_Expect_double_dots()
        {
            // Arrange
            var teamNameToArnRoleNameConverter = new TeamNameToTeamRoleArnConverter("123456789012");

            var teamNameWithSpaces = "team name with spaces";
            
            
            // Act
            var arnResult = teamNameToArnRoleNameConverter.Convert(teamNameWithSpaces);
            
            
            // Assert
            var expectedArn = "arn:aws:iam::123456789012:role/team..name..with..spaces";
            Assert.Equal(expectedArn, arnResult);
        }
    }
}