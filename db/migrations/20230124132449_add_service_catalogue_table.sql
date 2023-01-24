-- 2023-01-24 13:24:49 : add service catalogue table

CREATE TABLE public."ServiceCatalogue" (
    "Id" uuid NOT NULL,
    "Name" varchar(255) NOT NULL,
    "Description" varchar(1024) NOT NULL,
    "Created" timestamp NOT NULL,
    CONSTRAINT service_catalogue_pk PRIMARY KEY ("Id")
);