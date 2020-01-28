using System;
using DFDS.CapabilityService.WebApi.Domain.Exceptions;
using DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace DFDS.CapabilityService.WebApi.Infrastructure.Api
{
	public static class ExceptionToStatusCode
	{
		public static bool CanConvert(Exception exception, out IActionResult actionResult)
		{
			var convertResult = Convert(exception);
			actionResult = convertResult;

			return convertResult != null;
		}

		private static ActionResult Convert(Exception exception)
		{
			switch (exception)
			{
				case CapabilityDoesNotExistException _:
				case NotMemberOfCapabilityException _:
					return new NotFoundObjectResult(new {exception.Message});
				case CapabilityValidationException _:
					return new BadRequestObjectResult(new {exception.Message});
				case CapabilityWithSameNameExistException _:
				case DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Exceptions.TopicAlreadyExistException _:
				case DFDS.CapabilityService.WebApi.Domain.Exceptions.TopicAlreadyExistException _:
					return new ConflictObjectResult(new {exception.Message});
				case PartitionsCountNotAllowedException _:
					return new UnprocessableEntityObjectResult(new {exception.Message});
				default:
					return null;
			}
		}
	}
}
