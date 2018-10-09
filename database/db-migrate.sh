#!/bin/bash
# Run db-migrate from current folder with developer settings
node node_modules/db-migrate/bin/db-migrate \
--config database-definitions.json \
--env localhost-docker \
${@:1}