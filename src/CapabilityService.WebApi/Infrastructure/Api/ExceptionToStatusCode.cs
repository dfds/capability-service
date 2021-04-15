using System;
using DFDS.CapabilityService.WebApi.Domain.Exceptions;
using DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Exceptions;
using DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Models;
using Microsoft.AspNetCore.Http;
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
					return new ConflictObjectResult(new {exception.Message});
				case PartitionsCountNotAllowedException _:
				case TopicNameTooLongException _:
				case TopicDescriptionTooLongException _:
				case TopicNameTooShortException _:
					return new UnprocessableEntityObjectResult(new {exception.Message});
				case TopicDoesNotExistException _:
					return new BadRequestObjectResult(new {exception.Message});
				case ClusterNotSelectedException _:
					return new BadRequestObjectResult(new {exception.Message});
				case ClusterDoesNotExistException _:
					return new BadRequestObjectResult(new {exception.Message});
				case ClusterIsDisabledException _:
					return new BadRequestObjectResult(new {exception.Message});
				default:
					var resp = new JsonResult(new
					{
						ErrorMessage = exception.Message
					});
					resp.StatusCode = StatusCodes.Status500InternalServerError;
					return resp;
			}
		}
	}
}
