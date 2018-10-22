using System;
using DFDS.TeamService.WebApi.Features.AwsRoles;
using Xunit;

namespace DFDS.TeamService.Tests.Features.AwsRoles
{
    public class TeamNameToRoleNameConverterFacts
    {
        [Fact]
        public void GIVEN_simple_string_EXPECT_same_string()
        {
            // Arrange
            var teamNameToRoleNameConverter = new TeamNameToRoleNameConverter();

            var simpleTeamName = "team";
            
            
            // Act
            var arnResult = teamNameToRoleNameConverter.Convert(simpleTeamName);
            
            
            // Assert
            var roleName = simpleTeamName;
            Assert.Equal(roleName, arnResult);
        }


        [Fact]
        public void GIVEN_string_WITH_length_of_65_EXPECT_Exception()
        {
            // Arrange
            var teamNameToRoleNameConverter = new TeamNameToRoleNameConverter();
            
            var longTeamName = (new Guid().ToString() + new Guid() + new Guid()).Substring(0, 65);

            
            // Act
            // Assert
            Assert.Throws<ArgumentException>(() =>
                teamNameToRoleNameConverter.Convert(longTeamName)
            );
        }

        
        [Fact]
        public void GIVEN_string_WITH_plus_sign_EXPECT_Exception()
        {
            // Arrange
            var teamNameToArnRoleNameConverter = new TeamNameToRoleNameConverter();
            
            var teamNameWithPlusSign = "team+1";

            
            // Act
            // Assert
            Assert.Throws<ArgumentException>(() =>
                teamNameToArnRoleNameConverter.Convert(teamNameWithPlusSign)
            );
        }

        [Fact]
        public void GIVEN_spaces_Expect_plus_signs()
        {
            // Arrange
            var teamNameToArnRoleNameConverter = new TeamNameToRoleNameConverter();

            var teamNameWithSpaces = "team name with spaces";
            
            
            // Act
            var arnResult = teamNameToArnRoleNameConverter.Convert(teamNameWithSpaces);
            
            
            // Assert
            var expectedArn = "team+name+with+spaces";
            Assert.Equal(expectedArn, arnResult);
        }
    }
}