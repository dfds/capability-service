using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;

namespace DFDS.CapabilityService.WebApi.Infrastructure.Api
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
	public class BasicAuthAttribute : ActionFilterAttribute
	{
		public BasicAuthAttribute()
		{
		}

		public bool IsAuthorized(string validUserName, string validPassword, string basicAuthHeader)
		{
			if (String.IsNullOrEmpty(basicAuthHeader)) { return false; }

			var basicAuthHeaderWithoutBasic = basicAuthHeader
				.Replace("Basic ", "")
				.Replace("basic ", "");

			var decodedBasicAuth =
				System.Text.Encoding.ASCII.GetString(Convert.FromBase64String(basicAuthHeaderWithoutBasic));

			var usernameAndPasswordSplit = decodedBasicAuth.Split(':');
			var userName = usernameAndPasswordSplit[0];
			var password = usernameAndPasswordSplit[1];

			return userName == validUserName && password == validPassword;
		}

		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			var authOptions = filterContext.HttpContext.RequestServices
				.GetService<IOptions<AuthOptions>>()
				.Value;
			
			var authorizationHeaderValue = filterContext.HttpContext.Request.Headers["Authorization"].FirstOrDefault();

			var isAuthorized = IsAuthorized(
				authOptions.CAPABILITY_SERVICE_BASIC_AUTH_USER,
				authOptions.CAPABILITY_SERVICE_BASIC_AUTH_PASS,
				authorizationHeaderValue
			);

			if (isAuthorized) { return; }

			filterContext.Result = new UnauthorizedResult();
		}
	}
}
