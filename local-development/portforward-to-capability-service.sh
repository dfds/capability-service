#!/bin/bash

if [ "$1" == "stop" ]; then
    kill -9 $(lsof -i:a
 -t)
    echo "Forwarding stopped"
else
    kubectl -n selfservice port-forward service/capability-service 50900:80 &
fi

