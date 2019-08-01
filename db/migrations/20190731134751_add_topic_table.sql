-- 2019-07-31 13:47:51 : add topic table

CREATE TABLE public."Topic" (
    "Id" uuid NOT NULL,
    "Name" varchar(255) NOT NULL,
    "Description" varchar(1024) NOT NULL,
    "IsPrivate" boolean NOT NULL,
    "CapabilityId" uuid NOT NULL,
    CONSTRAINT topic_pk PRIMARY KEY ("Id")
);