using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DFDS.CapabilityService.WebApi.Domain.Models;
using DFDS.CapabilityService.WebApi.Domain.Repositories;

namespace DFDS.CapabilityService.WebApi.Application
{
    public interface ITopicApplicationService
    {
        Task<IEnumerable<Topic>> GetAllTopics(Guid capabilityId);
    }

    public class TopicApplicationService : ITopicApplicationService
    {
        private readonly ITopicRepository _topicRepository;

        public TopicApplicationService(ITopicRepository topicRepository)
        {
            _topicRepository = topicRepository;
        }

        public Task<IEnumerable<Topic>> GetAllTopics(Guid capabilityId) => _topicRepository.GetByCapability(capabilityId);
    }
}