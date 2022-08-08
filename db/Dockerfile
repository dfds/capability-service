FROM alpine:latest

RUN apk --update add postgresql-client ca-certificates tzdata && rm -rf /var/cache/apk/*

# SSL
RUN wget https://s3.amazonaws.com/rds-downloads/rds-combined-ca-bundle.pem -O /tmp/rds-combined-ca-bundle.pem \
    && mv /tmp/rds-combined-ca-bundle.pem /usr/local/share/ca-certificates/rds-combined-ca-bundle.crt \
    && update-ca-certificates

ENV PGSSLROOTCERT /etc/ssl/certs/ca-certificates.crt
ENV PGSSLMODE verify-full
ENV PGPORT 5432

WORKDIR /db

COPY . .

RUN chmod +x init.sh

#Non-root user settings
ARG USERNAME=capService
ARG USER_UID=1000
ARG USER_GID=$USER_UID

RUN addgroup -g $USER_GID -S $USERNAME && \
    adduser -u $USER_UID -S $USERNAME -G $USERNAME


USER $USERNAME

ENTRYPOINT [ "./init.sh" ]