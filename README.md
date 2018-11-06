[![Build Status](https://dfds.visualstudio.com/DevelopmentExcellence/_apis/build/status/team-service-CI?branch=master)](https://dfds.visualstudio.com/DevelopmentExcellence/_build/latest?definitionId=901&branch=master)[![Release Status](https://dfds.vsrm.visualstudio.com/_apis/public/Release/badge/ace5e409-c242-4356-93f4-23c53a3dc87b/35/57)](https://dfds.visualstudio.com/DevelopmentExcellence/_release?definitionId=35&_a=releases)

# Team service
Owns team context.
Creates AwsConsole urls.
Assigns roles to teams.

## Init 
What do you need to run this

## Deployment
How to deploy this

## Pull request and dev stuff

## Database

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

