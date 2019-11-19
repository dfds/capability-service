using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DFDS.CapabilityService.Tests.Infrastructure.Authentication
{
	public class MockAuthenticationHandler : AuthenticationHandler<MockAuthenticationSchemeOptions>
	{
		public MockAuthenticationHandler(
			IOptionsMonitor<MockAuthenticationSchemeOptions> options, ILoggerFactory logger,
			UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
		{
		}

		protected override Task<AuthenticateResult> HandleAuthenticateAsync()
		{
			var authenticationTicket = new AuthenticationTicket(
				new ClaimsPrincipal(Options.Identity),
				MockAuthenticationSchemeOptions.SchemeName);

			return Task.FromResult(AuthenticateResult.Success(authenticationTicket));
		}
	}
}
