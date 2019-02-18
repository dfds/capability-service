[![Build Status](https://dfds.visualstudio.com/DevelopmentExcellence/_apis/build/status/capability-service-CI?branch=master)](https://dfds.visualstudio.com/DevelopmentExcellence/_build/latest?definitionId=901&branch=master)[![Release Status](https://dfds.vsrm.visualstudio.com/_apis/public/Release/badge/ace5e409-c242-4356-93f4-23c53a3dc87b/35/57)](https://dfds.visualstudio.com/DevelopmentExcellence/_release?definitionId=35&_a=releases)
# DFDS devex "Capability Service"
Owns mapping between users (members) to capabilities and to cloud resources. We call this the context.

Offers some services for (/by extension of) managing the above responsibility, as well as some closely related workflow support services.

- Owns capability context
- Creates AwsConsole urls to allow users to directly log into an AWS Console in a role that matches their permissions
- Assigns roles to capabilities

# Getting started
Prerequisites:

1. (For Windows) Admin account on the local machine
2. Git >2.1
3. dotnet core ~2.1
4. NodeJS/npm ~6.4 or yarn ~1.12
5. Docker ~2.0
6. Docker-compose ~1.0
7. aws-cli ~1.16
8. Kubectl 1.12 (really it has to match both the cluster server version and our support. We've tested 1.12)
9. (Non-software) Credentials and URLs, see the env vars section below

Other versions may work but this setup is what we test on.

Commandline and environment examples below are based on a Bash shell but it should be directly translatable to other systems and shells.

## Init
Read pipeline.sh, and mostly ignore it. Once that's done, you need to set up the environment.

There is a Docker and docker-compose file available which brings up just the development database. Run `docker-compose up` from the project root to get Postgres up. Once it runs, you can continue with the next section.

## Running Capability Service
First restore dependencies by runing the `./pipeline` script located in the repository root or by navigating 
to the `./src` folder and run `dotnet restore` like shown below:

### Pipeline script
```bash
./pipeline.sh
```
### Manual restore
```bash
cd src
dotnet restore
````

Then the application can be executed by the following (navigate to the `./src` folder):
```bash
dotnet run --project CapabilityService.WebApi/
````

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

# Domain Events

Events are published on the Kafka topic `build.capabilities`. Currently the following events are published:

**Type:** `capabilitycreated`

**Occures:** when a new capability has been created

**Example**:
```json
{
	"messageId": "ab509002-b295-46c5-80c0-3f0178174927",
	"type": "capabilitycreated",
	"data": {
		"capabilityId": "2ca6bf98-d30d-4cf7-9adc-168a1e9c9849",
		"capabilityName": "Misser"
	}
}
```
---
**Type:** `memberjoinedcapability`

**Occures:** when a member joins a capability

**Example**:
```json
{
	"messageId": "f791bc18-6eec-4e1c-ba48-c384b174f961",
	"type": "memberjoinedcapability",
	"data": {
		"capabilityId": "0d03e3ad-2118-46b7-970e-0ca87b59a202",
		"memberEmail": "johndoe@jdog.com"
	}
}
```

### Migration in Kubernetes

**TODO**: here we should add the things we discussed at the white-board session on Wednesday the 17th of October.

