namespace DFDS.CapabilityService.WebApi.Infrastructure.Api.DTOs
{
    public class MessageContract
    {
        public string Type { get; set; }
        public string Content { get; set; }
        public string Description { get; set; }

        public static MessageContract CreateFrom(Domain.Models.MessageContract messageContract)
        {
            return new MessageContract
            {
                Type = messageContract.Type,
                Content = messageContract.Content,
                Description = messageContract.Description
            };
        }
    }
}