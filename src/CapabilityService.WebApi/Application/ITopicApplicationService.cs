using System;
using System.Threading.Tasks;

namespace DFDS.CapabilityService.WebApi.Application
{
    public interface ITopicApplicationService
    {
        Task UpdateMessageContract(Guid topicId, string type, string description, string content);
        Task RemoveMessageContract(Guid topicId, string type);
        Task UpdateTopic(Guid topicId, string name, string description, bool isPrivate);
    }
}