﻿using DFDS.CapabilityService.Tests.Infrastructure.Authentication;
using DFDS.CapabilityService.WebApi;
using DFDS.CapabilityService.WebApi.Infrastructure.Api;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DFDS.CapabilityService.Tests.TestDoubles
{
	public class TestAuthStartUp : Startup
	{
		public TestAuthStartUp(IConfiguration configuration) : base(configuration)
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

			var authOptions = Options.Create(new AuthOptions
			{
				CAPABILITY_SERVICE_BASIC_AUTH_USER_AND_PASS = BasicAuthCredentials.BASIC_AUTHENTICATION_USER_AND_PASS
			});
			services.AddSingleton<IOptions<AuthOptions>>(authOptions);
		}
	}
}
