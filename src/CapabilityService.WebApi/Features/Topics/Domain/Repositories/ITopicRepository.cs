using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DFDS.CapabilityService.WebApi.Features.Topics.Domain.Models;

namespace DFDS.CapabilityService.WebApi.Features.Topics.Domain.Repositories
{
    public interface ITopicRepository
    {
        Task<IEnumerable<Topic>> GetAll();
        Task<IEnumerable<Topic>> GetByCapability(Guid capabilityId);
        Task Add(Topic topic);
        Task<Topic> Get(Guid id);
        Task<IEnumerable<Topic>> FindBy(string name);
    }
}