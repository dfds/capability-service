using System;
using DFDS.CapabilityService.WebApi.Domain.Events;
using DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Models;
using Newtonsoft.Json;

namespace DFDS.CapabilityService.WebApi.Features.Kafka.Domain.Events
{
	public class TopicCreated : IDomainEvent
	{
		public TopicCreated(Topic topic)
		{
			Name = topic.Name;
			Description = topic.Description;
			CapabilityId = topic.CapabilityId;
			Partitions = topic.Partitions;
		} 
		
		[JsonConverter(typeof(TopicNameConverter))]
		public TopicName Name { get; private set; }
		public string Description { get; private set; }
		public Guid CapabilityId { get; private set; }
		public int Partitions { get; private set; }
	}


	public class TopicNameConverter : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			var topicName = value as TopicName;
			writer.WriteValue(topicName.Name);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}

		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(TopicName);
		}
	}
}
