using System;

namespace DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Exceptions
{
	public class TopicIdBadException  :Exception
	{
		public TopicIdBadException(string id) : base($"The id: '{id}' is not valid for topics the format must be 32 hex digits grouped into segments of 8-4-4-4-12, example: '879f78c9-39f3-45a4-852c-f35ab2f3844b'"){}
	}
}
