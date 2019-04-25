-- 2019-04-24 14:33:45 : add context

CREATE TABLE public."Context" (
    "Id" uuid NOT NULL,
    "Name" varchar(255) NOT NULL,
    "CapabilityId" uuid NOT NULL,
    CONSTRAINT context_pk PRIMARY KEY ("Id")
);