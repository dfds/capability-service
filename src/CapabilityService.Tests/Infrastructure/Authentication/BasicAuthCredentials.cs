using System;
using System.Net.Http.Headers;
using System.Text;

namespace DFDS.CapabilityService.Tests.Infrastructure.Authentication
{
	public static class BasicAuthCredentials
	{
		public const string BASIC_AUTHENTICATION_USER_AND_PASS = "user:thisisindeedapassword";
		public static AuthenticationHeaderValue BASIC_AUTHENTICATION_HEADER_VALUE = 
			new AuthenticationHeaderValue(
				"Basic", 
				Convert.ToBase64String(
					Encoding.ASCII.GetBytes(BASIC_AUTHENTICATION_USER_AND_PASS)
				)
			);
		


	}
}
