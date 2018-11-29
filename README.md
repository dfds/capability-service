[![Build Status](https://dfds.visualstudio.com/DevelopmentExcellence/_apis/build/status/team-service-CI?branch=master)](https://dfds.visualstudio.com/DevelopmentExcellence/_build/latest?definitionId=901&branch=master)[![Release Status](https://dfds.vsrm.visualstudio.com/_apis/public/Release/badge/ace5e409-c242-4356-93f4-23c53a3dc87b/35/57)](https://dfds.visualstudio.com/DevelopmentExcellence/_release?definitionId=35&_a=releases)
# DFDS devex "TeamService"
Owns mapping between users (members) to teams and to cloud resources. We call this the context.

Offers some services for (/by extension of) managing the above responsibility, as well as some closely related workflow support services.

- Owns team context
- Creates AwsConsole urls to allow users to directly log into an AWS Console in a role that matches their permissions
- Assigns roles to teams

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

## Running Teamservice

Depending on what you need to do, you'll need at least the first five variables below. It is often convenient to put these into a script and then `source awscredentials.sh` so you have them in your context. Powershell users can replace `EXPORT[space]` with `$`. There is also a launchSettings.json file but this file may not contain confidential keys.

```bash
export AWS_ACCESS_KEY_ID="[REDACTED]"
export AWS_ACCOUNT_ID="[REDACTED]"
export AWS_DEFAULT_REGION="[REDACTED]"
export AWS_SECRET_ACCESS_KEY="[REDACTED]"
# to use the local DB. Production database would typically use SSL, and in this case remember to add ;SSL Mode=Require to the conn. string (and install the CA)
export TEAM_DATABASE_CONNECTIONSTRING="User ID=postgres;Password=p;Host=localhost;Port=5432;Database=teamservice"

export AWS_COGNITO_ACCESS_KEY="[REDACTED]"
export AWS_COGNITO_IDENTITY_POOL_ID="[REDACTED]"
export AWS_COGNITO_LOGIN_PROVIDER_NAME="[REDACTED]"
export AWS_COGNITO_SECRET_ACCESS_KEY="[REDACTED]"
export AWS_COGNITO_USER_POOL_ID="[REDACTED]"
export BLASTER_COGNITO_CLIENT_ID="[REDACTED]"
export BLASTER_COGNITO_CLIENT_SECRET="[REDACTED]"
export BLASTER_COGNITO_POOL_ID="[REDACTED]"
export BLASTER_COGNITO_REGION="[REDACTED]"
```


Now run TeamService:

```bash
cd src
npm install
dotnet restore Blaster.sln
dotnet watch run # or just run
````

### Development caveats
Most code correctly handles dev vs production mode, however there is a known issue with the TeamCreatedSubcriber event handler. Regardless of environment, it attempts to persist role changes. If this is a problem, comment out the `await _identityClient.PutRoleAsync(domainEvent.TeamName);` line in that file.

## Deployment
FIXME How to deploy this FIXME

## Pull request and dev stuff
Pull requests through Github are welcome.

## Database
The database will initially start as empty. The image is constructed so that files can be added through the command below, and these will be run in date order (at least if you name the file right).

### Local Development

To add a migration, run:

```sh
./add-migration.sh create team table
```

Will create an empty migration file (e.g. `20181017194326_create_team_table.sql`) in the `./db/migrations` folder. The file will be prefixed with YYMMDDHHMMSS.

To bring up a local postgres database with all migration scripts applied against it, set the environment variables in `docker-compose.yml` as needed (or use defaults), and run:

```sh
docker-compose up --build
```

After adding new migrations, run `docker-compose down` and re-run the above command.

### Migration in Kubernetes

**TODO**: here we should add the things we discussed at the white-board session on Wednesday the 17th of October.

