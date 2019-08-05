using System;
using System.Threading.Tasks;
using DFDS.CapabilityService.WebApi.Domain.Exceptions;
using DFDS.CapabilityService.WebApi.Domain.Repositories;

namespace DFDS.CapabilityService.WebApi.Application
{
    public class TopicApplicationService : ITopicApplicationService
    {
        private readonly ITopicRepository _topicRepository;

        public TopicApplicationService(ITopicRepository topicRepository)
        {
            _topicRepository = topicRepository;
        }

        public async Task AddMessageContract(Guid topicId, string type, string description, string content)
        {
            var topic = await _topicRepository.Get(topicId);

            if (topic == null)
            {
                throw new TopicDoesNotExistException(topicId);
            }

            topic.AddMessageContract(type, description, content);
        }

        public async Task RemoveMessageContract(Guid topicId, string type)
        {
            var topic = await _topicRepository.Get(topicId);

            if (topic == null)
            {
                throw new TopicDoesNotExistException(topicId);
            }

            topic.RemoveMessageContract(type);
        }
    }
}