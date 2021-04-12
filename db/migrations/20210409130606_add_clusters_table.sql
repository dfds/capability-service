-- 2021-04-09 13:06:06 : add clusters table

CREATE TABLE public."KafkaCluster" (
    "Id" uuid NOT NULL,
    "ClusterId" varchar(255) NOT NULL,
    "Name" varchar(1024) NOT NULL,
    "Description" varchar(8192),
    "Enabled" boolean NOT NULL,
    CONSTRAINT cluster_pk PRIMARY KEY ("Id")
);