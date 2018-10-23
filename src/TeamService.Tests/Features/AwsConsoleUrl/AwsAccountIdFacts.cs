using System;
using DFDS.TeamService.WebApi.Features.AwsConsoleLogin;
using Xunit;

namespace DFDS.TeamService.Tests.Features.AwsConsoleUrl
{
    public class AwsAccountIdFacts
    {
        [Fact]
        public void GIVEN_letters_EXPECT_exception()
        {
            // Arrange
            var accountId = "abc456789012";

            
            // Act
            // Assert
            Assert.Throws<ArgumentException>(() => new AwsAccountId(accountId));
        }
        
        
        [Fact]
        public void GIVEN_13_digits_EXPECT_exception()
        {
            // Arrange
            var accountId = "1234567890123";

            
            // Act
            // Assert
            Assert.Throws<ArgumentException>(() => new AwsAccountId(accountId));
        }
        
        
        [Fact]
        public void GIVEN_11_digits_EXPECT_exception()
        {
            // Arrange
            var accountId = "12345678901";

            
            // Act
            // Assert
            Assert.Throws<ArgumentException>(() => new AwsAccountId(accountId));
        }
        
          
        [Fact]
        public void GIVEN_12_digits_EXPECT_sucess()
        {
            // Arrange
            var accountId = "123456789012";

            
            // Act
            var awsAccountId = new AwsAccountId(accountId);

            
            // Assert
            Assert.Equal(accountId, awsAccountId.ToString());
        }
    }
}