using System;
using System.Linq;
using System.Threading.Tasks;
using DFDS.CapabilityService.WebApi.Features.Topics.Domain.Exceptions;
using DFDS.CapabilityService.WebApi.Features.Topics.Domain.Repositories;

namespace DFDS.CapabilityService.WebApi.Features.Topics.Application
{
    public class TopicApplicationService : ITopicApplicationService
    {
        private readonly ITopicRepository _topicRepository;

        public TopicApplicationService(ITopicRepository topicRepository)
        {
            _topicRepository = topicRepository;
        }

        public async Task UpdateMessageContract(Guid topicId, string type, string description, string content)
        {
            var topic = await _topicRepository.Get(topicId);

            if (topic == null)
            {
                throw new TopicDoesNotExistException(topicId);
            }

            if (topic.HasMessageContract(type))
            {
                topic.UpdateMessageContract(type, description, content);
            }
            else
            {
                topic.AddMessageContract(type, description, content);
            }
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

        public async Task UpdateTopic(Guid topicId, string name, string description, bool isPrivate)
        {
            var topic = await _topicRepository.Get(topicId);
            if (topic == null)
            {
                throw new TopicDoesNotExistException(topicId);
            }

            var topicsWithSameName = await _topicRepository.FindBy(name);
            var isTopicNameTakenByAnotherTopic = topicsWithSameName
                .Where(x => x.Id != topicId)
                .Any();

            if (isTopicNameTakenByAnotherTopic)
            {
                throw new TopicAlreadyExistException($"A topic with the name \"{name}\" already exist.");
            }

            topic.Name = name;
            topic.Description = description;
            topic.IsPrivate = isPrivate;
        }
    }
}