using DFDS.CapabilityService.WebApi.Infrastructure.Api;
using Xunit;

namespace DFDS.CapabilityService.Tests.Infrastructure.Api
{
	public class TestBasicAuthAttribute
	{
		[Fact]
		public void AHeaderCanBeValidated()
		{
			// Arrange
			var authHeader = "Basic dXNlcjp0aGlzaXNpbmRlZWRhcGFzc3dvcmQ=";
			var validUserName = "user";
			var validPassword = "thisisindeedapassword";
			var basicAuthAttribute = new BasicAuthAttribute();
			
			// Act
			var isAuthorized = basicAuthAttribute.IsAuthorized(
				validUserName, 
				validPassword, 
				authHeader
			);
			
			// Assert
			Assert.True(isAuthorized);
			
		}
		
		[Fact]
		public void GivenNullHeaderExpectFalse()
		{
			// Arrange
			string authHeader = null;
			var validUserName = "user";
			var validPassword = "thisisindeedapassword";
			var basicAuthAttribute = new BasicAuthAttribute();
			
			// Act
			var isAuthorized = basicAuthAttribute.IsAuthorized(
				validUserName, 
				validPassword, 
				authHeader
			);
			
			// Assert
			Assert.False(isAuthorized);
			
		}
	}
}
