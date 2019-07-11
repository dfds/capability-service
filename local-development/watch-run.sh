#!/bin/bash
ASPNETCORE_ENVIRONMENT="Development" \
CAPABILITYSERVICE_DATABASE_CONNECTIONSTRING="User ID=postgres;Password=p;Host=localhost;Port=5432;Database=capabilitydb;" \
CAPABILITY_SERVICE_KAFKA_BOOTSTRAP_SERVERS=localhost:9092 \
CAPABILITY_SERVICE_KAFKA_TOPIC_CAPABILITY="build.capabilities" \
CAPABILITY_SERVICE_HUMAN_LOG="true" \
dotnet watch --project ./../src/CapabilityService.WebApi/CapabilityService.WebApi.csproj \
run --server.urls "http://*:50900" 

# For some reason, adding this parameter suppresses Serilog output in objects with constructor DI??: --no-launch-profile