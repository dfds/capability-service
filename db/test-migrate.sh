#!/bin/bash

docker build -t migrate .

docker run -it --rm \
    -e PGDATABASE=teamservice \
    -e PGHOST=ded-teamservicedb.czgqd5dy5owa.eu-central-1.rds.amazonaws.com \
    -e PGPORT=1433 \
    -e PGUSER=${PGUSER} \
    -e PGPASSWORD=${PGPASSWORD} \
    migrate
