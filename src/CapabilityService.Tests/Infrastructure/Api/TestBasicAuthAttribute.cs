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
			var validUserAndPassword = "user:thisisindeedapassword";
			var basicAuthAttribute = new BasicAuthAttribute();
			
			// Act
			var isAuthorized = basicAuthAttribute.IsAuthorized(
				validUserAndPassword,
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
			var validUserAndPassword = "user:thisisindeedapassword";
			var basicAuthAttribute = new BasicAuthAttribute();
			
			// Act
			var isAuthorized = basicAuthAttribute.IsAuthorized(
				validUserAndPassword,
				authHeader
			);
			
			// Assert
			Assert.False(isAuthorized);
			
		}
	}
}
