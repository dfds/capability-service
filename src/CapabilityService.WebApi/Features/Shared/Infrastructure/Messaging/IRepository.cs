using System.Collections.Generic;
using System.Threading.Tasks;

namespace DFDS.CapabilityService.WebApi.Features.Shared.Infrastructure.Messaging
{
    public interface IRepository<T> where T:class
    {
        Task Add(IEnumerable<T> obj);

        Task<IEnumerable<T>> GetAll();
    }
}