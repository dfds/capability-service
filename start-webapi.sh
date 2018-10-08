#!/bin/bash

cd src/TeamService.WebApi

AWS_COGNITO_ACCESS_KEY="" \
AWS_COGNITO_SECRET_ACCESS_KEY="" \
AWS_COGNITO_USER_POOL_ID="" \
AWS_COGNITO_LOGIN_PROVIDER_NAME="" \
AWS_COGNITO_IDENTITY_POOL_ID="" \
ASPNETCORE_URLS=http://+:8080 \
dotnet watch run --no-launch-profile

SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null && pwd )"
cd SCRIPT-DIR
