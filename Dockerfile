FROM microsoft/dotnet:2.1.1-aspnetcore-runtime-stretch-slim

WORKDIR /app
COPY ./output ./

ENV ASPNETCORE_URLS=http://+:80

ENTRYPOINT [ "dotnet", "TeamService.WebApi.dll" ]