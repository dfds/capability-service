using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace DFDS.CapabilityService.Tests.Infrastructure.Authentication
{
	public class MockAuthenticationSchemeOptions : AuthenticationSchemeOptions
	{
		public const string SchemeName = "MockAuth";

		public virtual ClaimsIdentity Identity { get; } = new ClaimsIdentity(new[]
		{
			new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", Guid.NewGuid().ToString())

		}, "test");
	}
}
