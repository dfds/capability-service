using System;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DFDS.CapabilityService.Tests.Infrastructure.Authentication
{
	public class LocalAuthenticationHandler : AuthenticationHandler<LocalAuthenticationSchemeOptions>
	{

		public LocalAuthenticationHandler(
			IOptionsMonitor<LocalAuthenticationSchemeOptions> options, ILoggerFactory logger,
			UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
		{
		}

		protected override Task<AuthenticateResult> HandleAuthenticateAsync()
		{
			var authenticationTicket = new AuthenticationTicket(
				new ClaimsPrincipal(Options.Identity),
				LocalAuthenticationSchemeOptions.SchemeName);

			return Task.FromResult(AuthenticateResult.Success(authenticationTicket));
		}
	}

	public class LocalAuthenticationSchemeOptions : AuthenticationSchemeOptions
	{
		public const string SchemeName = "LocalAuth";

		public virtual ClaimsIdentity Identity { get; } = new ClaimsIdentity(new[]
		{
			new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", Guid.NewGuid().ToString())

		}, "test");
	}
}
