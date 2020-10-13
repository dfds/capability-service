[![Build Status](https://dfds.visualstudio.com/DevelopmentExcellence/_apis/build/status/capability-service-CI?branch=master)](https://dfds.visualstudio.com/DevelopmentExcellence/_build/latest?definitionId=901&branch=master) [![Release Status](https://dfds.vsrm.visualstudio.com/_apis/public/Release/badge/ace5e409-c242-4356-93f4-23c53a3dc87b/35/64)](https://dfds.visualstudio.com/DevelopmentExcellence/_release?definitionId=35&_a=releases)
# Capability Service
Owns mapping between users (members) to capabilities and to cloud resources. We call this the context.

## Getting started

### Prerequisites:
1. dotnet core 2.2 sdk
2. docker
3. docker-compose
4. bash

### Directory outline
The most **significant** items found in the repository/directory root are:
```text
.
├── Dockerfile
├── README.md
├── add-migration.sh
├── api-contracts/
├── db/
├── docker-compose.yml
├── docs/
├── fake_dependencies/
├── k8s/
├── pipeline.sh
└── src/
```
_Please note: you'd might also find other items in the repository/directory root._

Here is a small description for each of the items:

| Item | Description |
|------|-------------|
| Dockerfile | Describes how the runtime environment for the actual application should look like. |
| README.md | _This_ readme file |
| add-migration.sh | Util for quickly generating a database migration script that follows default conventions. Run it like this: `./add-migration.sh "<small description of you change here>"`. |
| api-contracts/ | Directory that contains the current OpenApi contract(s) exposed from the service. |
| db/ | Directory that contains all things related to the database setup e.g. Dockerfile for init migration container, migration script etc. |
| docker-compose.yml | Docker-compose file for bringing all external dependencies up (some in a 'faked-out' version). |
| docs/ | Directory for any documents that take part in documenting the service e.g. domain events. |
| fake_dependencies/ | Directory containing source code for 'faked-out' dependencies (often written in nodejs). |
| k8s/ | Directory containing all Kubernetes manifests used to describe the desired runtime state for the service in Kubernetes. |
| pipeline.sh | _The_ shell script used to implement the _continous integration_ pipeline. The script also generates a docker container image as a deployment artifact. |
| src/ | The _main_ directory for all the source code for the service. |

### Running the service
First restore dependencies by runing the `./pipeline` script located in the repository root or by navigating to the `./src` folder and run `dotnet restore` like shown below:

#### Pipeline script
```bash
./pipeline.sh
```
#### Manual restore
```bash
cd src
dotnet restore
```

#### Start the application
Then the application can be executed by the following (navigate to the `./src` folder):
```bash
dotnet run --project CapabilityService.WebApi/
```
The application can also be executed by running the bash script `watch-run.sh` in the folder `local-development` running the application this way is good for development on your own machine.

## Database
The database will initially start as empty. The image is constructed so that files can be added through the command below, and these will be run in date order (at least if you name the file right).

### Local Development

To add a migration, run:

```sh
./add-migration.sh create capability table
```

Will create an empty migration file (e.g. `20181017194326_create_capability_table.sql`) in the `./db/migrations` folder. The file will be prefixed with YYMMDDHHMMSS.

To bring up a local postgres database with all migration scripts applied against it, set the environment variables in `docker-compose.yml` as needed (or use defaults), and run:

```sh
docker-compose up --build
```

After adding new migrations, run `docker-compose down` and re-run the above command.

## Domain Events
For information about which domain events are published refere to the [domain events document](docs/domain_events.md).

## Capability persistance layers

Information about capabilities is persisted in layers by applying the the [decorator pattern](https://en.wikipedia.org/wiki/Decorator_pattern) to the ICapabilityApplicationService interface.

The layers act in the following order:

1. The CapabilityApplicationService will use the ICapabilityRepository to attach a Capability to the CapabilityServiceDbContext on CreateCapability().
1. The CapabilityOutboxEnabledDecorator will use the db contexts ChangeTracker to find IAggregateDomainEvents and pass them to the Outbox. The outbox will create DomainEventEnvelopes and call the IRepository\<DomainEventEnvelope\> with the DomainEventEnvelopes which will be added to the db context,

1. CapabilityTransactionalDecorator will:
    1. Create a new transaction
    1. Persist all changes made to the CapabilityServiceDbContext with the call of SaveChangesAsync.
    1. Commit the created transaction

The effect of this is that no change to database will be made if any of the operations the CapabilityApplicationService or CapabilityOutboxEnabledDecorator fails. Meaning you will not get a inconsistent state where changes made to the capability object is persisted but not reflected in a persisted DomainEventEnvelop.


## Deployment prerequisites

If the scoped service account is missing for deployment, see https://wiki.dfds.cloud/en/teams/devex/selfservice/Kubernetes-selfservice-deployment-setup
