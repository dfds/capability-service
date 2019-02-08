-- 2019-02-08 16:11:44 : create table for domain events

CREATE TABLE public."DomainEvent" (
    "EventId" uuid NOT NULL,
    "AggregateId" varchar(255) NOT NULL,
    "Type" varchar(255) NOT NULL,
    "Format" varchar(255) NOT NULL,
    "Data" text NOT NULL,
    "Created" timestamp NOT NULL,
    "Sent" timestamp NULL,
    CONSTRAINT domainevent_pk PRIMARY KEY ("EventId")
);
