using System;
using System.Threading.Tasks;

namespace DFDS.CapabilityService.WebApi.Application
{
    public interface ITopicApplicationService
    {
        Task AddMessageContract(Guid topicId, string type, string description, string content);
        Task RemoveMessageContract(Guid topicId, string type);
    }
}