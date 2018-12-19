-- 2018-10-17 19:43:26 : create team table

CREATE TABLE public."Team" (
    "Id" uuid NOT NULL,
    "Name" varchar(255) NOT NULL,
    CONSTRAINT team_pk PRIMARY KEY ("Id")
);
