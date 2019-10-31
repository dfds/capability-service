using System.Collections.Generic;
using System.Linq;

namespace CapabilityService.IntegrationTests.Features.Capabilities.Infrastructure.Api.Model
{
    public class ItemsEnvelope<T>
       {
           public T[] Items { get; }
   
           public ItemsEnvelope(IEnumerable<T> items)
           {
               Items = items.ToArray();
           }
       }
}