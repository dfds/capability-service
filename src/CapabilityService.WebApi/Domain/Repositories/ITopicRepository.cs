using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DFDS.CapabilityService.WebApi.Domain.Models;

namespace DFDS.CapabilityService.WebApi.Domain.Repositories
{
    public interface ITopicRepository
    {
        Task<IEnumerable<Topic>> GetAll();
        Task<IEnumerable<Topic>> GetByCapability(Guid capabilityId);
        Task Add(Topic topic);
        Task<Topic> Get(Guid id);
    }
}