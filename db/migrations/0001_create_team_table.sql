-- 2018-10-16 18:26:55 : initial database structure

CREATE TABLE public."Team" (
    "Id" uuid NOT NULL,
    "Name" varchar(255) NOT NULL,
    "Department" varchar(255) NULL,
    CONSTRAINT team_pk PRIMARY KEY ("Id")
);
