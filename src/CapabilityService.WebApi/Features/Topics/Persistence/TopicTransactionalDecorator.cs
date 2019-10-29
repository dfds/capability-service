using System;
using System.Threading.Tasks;
using DFDS.CapabilityService.WebApi.Features.Capabilities.Infrastructure.Persistence;
using DFDS.CapabilityService.WebApi.Features.Shared.Infrastructure.Persistence;
using DFDS.CapabilityService.WebApi.Features.Topics.Application;

namespace DFDS.CapabilityService.WebApi.Features.Topics.Persistence
{
    public class TopicTransactionalDecorator : TransactionalDecoratorBase, ITopicApplicationService
    {
        private readonly ITopicApplicationService _inner;

        public TopicTransactionalDecorator(ITopicApplicationService inner, CapabilityServiceDbContext dbContext) : base(dbContext)
        {
            _inner = inner;
        }

        public Task UpdateMessageContract(Guid topicId, string type, string description, string content)
        {
            return ExecuteInTransaction(() =>_inner.UpdateMessageContract(topicId, type, description, content));
        }

        public Task RemoveMessageContract(Guid topicId, string type)
        {
            return ExecuteInTransaction(() => _inner.RemoveMessageContract(topicId, type));
        }

        public Task UpdateTopic(Guid topicId, string name, string description, bool isPrivate)
        {
            return ExecuteInTransaction(() => _inner.UpdateTopic(topicId, name, description, isPrivate));
        }
    }
}