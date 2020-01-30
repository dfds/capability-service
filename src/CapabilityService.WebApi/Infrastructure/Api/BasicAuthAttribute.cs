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
		
		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			var svc = filterContext.HttpContext.RequestServices;
			var req = filterContext.HttpContext.Request;
			var auth = req.Headers["Authorization"];
			var options = svc.GetService<IOptions<AuthOptions>>().Value;
			
			if (!String.IsNullOrEmpty(auth))
			{
				var cred = System.Text.Encoding.ASCII.GetString(Convert.FromBase64String(auth.First().Substring(6))).Split(':');
				var user = new { Name = cred[0], Pass = cred[1] };
				if (user.Name == options.CAPABILITY_SERVICE_BASIC_AUTH_USER && user.Pass == options.CAPABILITY_SERVICE_BASIC_AUTH_PASS) return;
			}
			filterContext.Result = new UnauthorizedResult();
		}
	}
	
	
}
