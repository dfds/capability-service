#!/bin/bash

kustomize build ../k8s-orchestration/overlays/for-development | \
tee /dev/tty | \
kubectl apply -f -
