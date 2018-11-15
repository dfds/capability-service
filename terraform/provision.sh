#!/bin/bash
##
## Runs terragrunt in Docker in order to provision the infrastructure. 
##
## usage: provision.sh [environment]
## 

[[ -n "${DEBUG}" ]] && set -o xtrace

readonly ENV=${1:?"environment not specified"}

set -o nounset
set -o errexit
set -o pipefail

##############################################################################
#
# Globals:
#   ENV
#   AWS_ACCESS_KEY_ID
#   AWS_SECRET_ACCESS_KEY
#   MASTER_PASSWORD
# Arguments:
#   command to pass to terragrunt (in Docker)
# Returns:
#   None
##############################################################################
docker_run() {
    docker run \
        -v ${PWD}/terraform:/terraform \
        -w "/terraform/${ENV}" \
        -e AWS_ACCESS_KEY_ID="${AWS_ACCESS_KEY_ID}" \
        -e AWS_SECRET_ACCESS_KEY="${AWS_SECRET_ACCESS_KEY}" \
        -e TF_VAR_master_password="${MASTER_PASSWORD}" \
        dfdsdk/terragrunt-runner \
        ${1} --terragrunt-non-interactive
}

# docker_run apply-all
docker_output=$(docker_run output-all)

echo $docker_output | sed -E 's/(.*) = (.*)/##vso[task.setvariable variable=\1]\2/g'
