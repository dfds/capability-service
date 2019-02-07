FROM microsoft/dotnet:2.1.1-aspnetcore-runtime-stretch-slim

# RUN apt-get update -y && apt-get upgrade -y 
# RUN apt-get install ca-certificates && rm -rf /var/cache/apk/*

# SSL
RUN curl -o /tmp/rds-combined-ca-bundle.pem https://s3.amazonaws.com/rds-downloads/rds-combined-ca-bundle.pem \
    && mv /tmp/rds-combined-ca-bundle.pem /usr/local/share/ca-certificates/rds-combined-ca-bundle.crt \
    && update-ca-certificates

WORKDIR /app
COPY ./output/app ./

ENV ASPNETCORE_URLS=http://+:80

ENTRYPOINT [ "dotnet", "CapabilityService.WebApi.dll" ]