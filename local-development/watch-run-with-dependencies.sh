#!/bin/bash
until docker-compose -f ../docker-compose.yml up | grep -m 1 "kafka entered RUNNING state"; do : sleep 1 ; done

./watch-run.sh