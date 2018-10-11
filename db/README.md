# Database Migrations Proposal

## Local Development Criteria

* Hassle-free (`docker run ...` / `docker-compose up`)
* Controlled by connection string (HOST,DATABASE,UID,PWD,...) through environment variables
* Create database for local usage
* Simple TSQL files for migrations/transitions

## Migration in Kubernetes

* Options 1:
    * run "positive" database transitions
    * deploy
    * run "negative" database transitions
    * deploy
    ---
    * use a k8s job for the migration?
    * break bigger migrations into multiple steps (nullable, back-filling, etc.)
    * would there be cases for deploying code before running migrations?

* init containers is not a serious contender

```sh
docker-compose -f docker-compose.local.yml up --build
```
