#!/bin/sh
docker-compose -f docker-compose.yml -f docker-compose.local-debug.yml up --build && docker-compose down
