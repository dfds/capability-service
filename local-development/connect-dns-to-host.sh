#!/bin/bash

docker run --hostname dns.mageddo \
--restart=unless-stopped \
-p 5380:5380 \
-v /var/run/docker.sock:/var/run/docker.sock \
-v /etc/resolv.conf:/etc/resolv.conf \
defreitas/dns-proxy-server