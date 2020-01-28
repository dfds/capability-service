using DFDS.CapabilityService.Tests.Infrastructure.Authentication;
using DFDS.CapabilityService.WebApi;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DFDS.CapabilityService.Tests.TestDoubles
{
	public class AlwaysAllowAuthStartUp : Startup
	{
		public AlwaysAllowAuthStartUp(IConfiguration configuration) : base(configuration)
		{
		}

		protected override void ConfigureAuth(IServiceCollection services)
		{
			// Disable authentication by replacing auth and challenge scheme

			services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = MockAuthenticationSchemeOptions.SchemeName;
				options.DefaultChallengeScheme = MockAuthenticationSchemeOptions.SchemeName;
			})
			.AddScheme<MockAuthenticationSchemeOptions, MockAuthenticationHandler>(MockAuthenticationSchemeOptions.SchemeName, null);
		}
	}
}
