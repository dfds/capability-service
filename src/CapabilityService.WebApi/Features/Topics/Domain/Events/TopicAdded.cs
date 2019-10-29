namespace DFDS.CapabilityService.WebApi.Domain.Events
{
    public class TopicAdded : IDomainEvent
    {
        public TopicAdded(string topicId, string topicName, string topicDescription, bool topicIsPrivate, string capabilityId)
        {
            TopicId = topicId;
            TopicName = topicName;
            TopicDescription = topicDescription;
            TopicIsPrivate = topicIsPrivate;
            CapabilityId = capabilityId;
        }

        public string TopicId { get; set; }
        public string TopicName { get; set; }
        public string TopicDescription { get; set; }
        public bool TopicIsPrivate { get; set; }
        public string CapabilityId { get; set; }
    }
}