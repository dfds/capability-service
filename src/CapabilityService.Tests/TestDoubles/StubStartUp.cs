using DFDS.CapabilityService.Tests.Infrastructure.Authentication;
using DFDS.CapabilityService.WebApi;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DFDS.CapabilityService.Tests.TestDoubles
{
	public class StubStartUp : Startup
	{
		public StubStartUp(IConfiguration configuration) : base(configuration)
		{
		}

		protected override void ConfigureAuth(IServiceCollection services)
		{
			services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = LocalAuthenticationSchemeOptions.SchemeName;
				options.DefaultChallengeScheme = LocalAuthenticationSchemeOptions.SchemeName;
			})
			.AddScheme<LocalAuthenticationSchemeOptions, LocalAuthenticationHandler>(LocalAuthenticationSchemeOptions.SchemeName, null);
		}
	}
}
