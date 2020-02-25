using System;
using System.Threading.Tasks;

namespace DFDS.CapabilityService.WebApi
{
	public static class TaskFactoryExtensions
	{
		public static void StartActionWithConsoleExceptions(Action action)
		{
			Task.Factory
				.StartNew(action)
				.ContinueWith(
					_ =>
					{
						foreach (var exception in _.Exception.InnerExceptions)
						{
							Console.WriteLine(exception);
						}
					},
					TaskContinuationOptions.OnlyOnFaulted
				);
		}
	}
}
