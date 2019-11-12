#!/bin/bash
#
# build.sh(1)
#

[[ -n $DEBUG ]] && set -x
set -eu -o pipefail

# build parameters
readonly REGION=${AWS_DEFAULT_REGION:-"eu-central-1"}
readonly IMAGE_NAME="ded/capability-service"
readonly DB_IMAGE_NAME="${IMAGE_NAME}/dbmigrations"
readonly BUILD_NUMBER=${1:-"N/A"}
readonly BUILD_SOURCES_DIRECTORY=${2:-${PWD}}

clean_output_folder() {
    rm -Rf output
    mkdir output
}

restore_dependencies() {
    echo "Restoring dependencies"
    dotnet restore CapabilityService.sln
}

run_tests() {
    echo "Running tests..."
    dotnet build -c Release CapabilityService.sln

    MSYS_NO_PATHCONV=1 dotnet test \
        --logger:"trx;LogFileName=testresults.trx" \
        src/CapabilityService.Tests/CapabilityService.Tests.csproj \
        /p:CollectCoverage=true \
        /p:CoverletOutputFormat=cobertura \
        '/p:Include="[CapabilityService.WebApi]*"'

    mv ./src/CapabilityService.Tests/coverage.cobertura.xml "${BUILD_SOURCES_DIRECTORY}/output/"
    mv ./src/CapabilityService.Tests/TestResults/testresults.trx "${BUILD_SOURCES_DIRECTORY}/output/"
}

publish_binaries() {
    echo "Publishing binaries..."
    dotnet publish -c Release -o ${BUILD_SOURCES_DIRECTORY}/output/app src/CapabilityService.WebApi/CapabilityService.WebApi.csproj
}

build_container_image() {
    echo "Building container images..."
    cd ./src/CapabilityService.WebApi
    docker build -t ${IMAGE_NAME} .

    cd ../..
    docker build -t ${DB_IMAGE_NAME} ./db
}

login_to_docker() {
    echo "Login to docker..."
    $(aws ecr get-login --no-include-email)
}

push_container_image() {
    # echo "Login to docker..."
    # $(aws ecr get-login --no-include-email)

    account_id=$(aws sts get-caller-identity --output text --query 'Account')
    image_name="${account_id}.dkr.ecr.${REGION}.amazonaws.com/${IMAGE_NAME}:${BUILD_NUMBER}"

    echo "Tagging container image..."
    docker tag ${IMAGE_NAME}:latest ${image_name}

    echo "Pushing container image to ECR..."
    docker push ${image_name}
}

push_dbmigration_container_image() {
    # echo "Login to docker..."
    # $(aws ecr get-login --no-include-email)

    account_id=$(aws sts get-caller-identity --output text --query 'Account')
    image_name="${account_id}.dkr.ecr.${REGION}.amazonaws.com/${DB_IMAGE_NAME}:${BUILD_NUMBER}"

    echo "Tagging container image..."
    docker tag ${DB_IMAGE_NAME}:latest ${image_name}

    echo "Pushing container image to ECR..."
    docker push ${image_name}
}

clean_output_folder



restore_dependencies
run_tests


publish_binaries



build_container_image

if [[ "${BUILD_NUMBER}" != "N/A" ]]; then
    login_to_docker
    push_container_image
    push_dbmigration_container_image
fi