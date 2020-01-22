-- 2020-01-20 05:45:51 : add kafka topic table


CREATE TABLE public."KafkaTopic" (
    "Id" uuid NOT NULL,
    "Name" varchar(255) NOT NULL,
    "Description" varchar(1024) NOT NULL,
    "CapabilityId" uuid NOT NULL,
    "Partitions" smallserial NOT NULL,
    CONSTRAINT kafaka_topic_pk PRIMARY KEY ("Id")
);