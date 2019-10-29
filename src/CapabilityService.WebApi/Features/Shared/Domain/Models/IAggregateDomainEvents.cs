using System.Collections.Generic;
using DFDS.CapabilityService.WebApi.Domain.Events;
using DFDS.CapabilityService.WebApi.Features.Shared.Domain.Events;

namespace DFDS.CapabilityService.WebApi.Features.Shared.Domain.Models
{
    public interface IAggregateDomainEvents
    {
        IEnumerable<IDomainEvent> DomainEvents { get; }
        void ClearDomainEvents();

        string GetAggregateId();
    }
}