#!/bin/bash
IAMROLESERVICE_URL=http://localhost:50800 \
ROLEMAPPERSERVICE_URL=http://localhost:50801 \
CAPABILITYSERVICE_DATABASE_CONNECTIONSTRING=ID=postgres;Password=p;Host=localhost;Port=5432;Database=capabilitydb; \
CAPABILITY_SERVICE_KAFKA_BOOTSTRAP_SERVERS=localhost:9092 \
dotnet watch --project ./../src/CapabilityService.WebApi/CapabilityService.WebApi.csproj run