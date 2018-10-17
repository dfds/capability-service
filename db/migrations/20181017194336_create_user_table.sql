-- 2018-10-17 19:43:36 : create user table

CREATE TABLE public."User" (
    "Id" varchar(255) NOT NULL,
    "Name" varchar(255) NOT NULL,
    "Email" varchar(255) NOT NULL,
    CONSTRAINT user_pk PRIMARY KEY ("Id")
);
