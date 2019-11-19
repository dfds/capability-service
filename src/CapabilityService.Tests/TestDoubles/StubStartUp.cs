using DFDS.CapabilityService.Tests.Infrastructure.Authentication;
using DFDS.CapabilityService.WebApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
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
			services.AddMvc(config =>
			{
				var policy = new AuthorizationPolicyBuilder()
					.AddAuthenticationSchemes(MockAuthenticationSchemeOptions.SchemeName)
					.RequireAuthenticatedUser()
					.Build();

				config.Filters.Add(new AuthorizeFilter(policy));
			});

			services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = MockAuthenticationSchemeOptions.SchemeName;
				options.DefaultChallengeScheme = MockAuthenticationSchemeOptions.SchemeName;
			})
			.AddScheme<MockAuthenticationSchemeOptions, MockAuthenticationHandler>(MockAuthenticationSchemeOptions.SchemeName, null);
		}
	}
}
