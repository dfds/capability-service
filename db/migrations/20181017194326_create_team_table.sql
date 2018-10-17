-- 2018-10-17 19:43:26 : create team table

CREATE TABLE public."Team" (
    "Id" uuid NOT NULL,
    "Name" varchar(255) NOT NULL,
    "Department" varchar(255) NULL,
    CONSTRAINT team_pk PRIMARY KEY ("Id")
);
