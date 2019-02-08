using System.Collections.Generic;
using DFDS.CapabilityService.WebApi.Domain.Events;

namespace DFDS.CapabilityService.WebApi.Domain.Models
{
    public interface IAggregateDomainEvents
    {
        IEnumerable<DomainEvent> DomainEvents { get; }
        void ClearDomainEvents();
    }
}