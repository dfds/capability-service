# Todo list

Make RootId a value object

/src/CapabilityService.WebApi/Infrastructure/Messaging/Outbox.cs
Rename variables to reflect types used

/src/CapabilityService.WebApi/Infrastructure/Persistence/CapabilityRepository.cs : 33
Remove SaveChangesAsync and let the transaction decorator take care of the save
Note:
We should probably look at implementing connection resilience our Db context @ https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency via execution strategies.

src/CapabilityService.WebApi/Application/CapabilityApplicationService.cs : 35
Use a capabilityName value type instead of initiating a Capability

src/CapabilityService.WebApi/Infrastructure/Api/CapabilityController.cs
return a 422 if the given id can not be parsed to a guid.

/src/CapabilityService.WebApi/Infrastructure/Api/CapabilityController.cs : 109 / 131
replace Guid.TryCatch with a value object
give propper response code if validation fails

Look into enforcing the Transaction scope by allowing SaveAsync more places or abandom the concept for something else.
This could be https://docs.microsoft.com/en-us/aspnet/mvc/overview/older-versions/getting-started-with-ef-5-using-mvc-4/implementing-the-repository-and-unit-of-work-patterns-in-an-asp-net-mvc-application
https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency#execution-strategies-and-transactions
