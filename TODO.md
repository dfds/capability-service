# Todo list

/src/CapabilityService.WebApi/Infrastructure/Api/CapabilityController.cs : 109 / 131
replace Guid.TryCatch with a value object
give propper response code if validation fails

/src/CapabilityService.WebApi/Infrastructure/Messaging/Outbox.cs
Rename variables to reflect types used

/src/CapabilityService.WebApi/Infrastructure/Persistence/CapabilityRepository.cs : 33
Remove SaveChangesAsync and let the transaction decorator take care of the save
