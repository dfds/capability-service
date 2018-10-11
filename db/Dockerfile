FROM alpine:3.7

RUN apk --update add postgresql-client ca-certificates && rm -rf /var/cache/apk/*
# COPY or WGET ./rds-ca-2015-eu-central-1.pem /usr/local/share/ca-certificates/.
# RUN update-ca-certificates

WORKDIR /db

COPY . ./

RUN chmod +x init.sh

ENTRYPOINT [ "./init.sh" ]
