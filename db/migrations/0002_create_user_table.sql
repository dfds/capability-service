-- 2018-10-16 18:26:55 : initial database structure

CREATE TABLE public."User" (
    "Id" varchar(255) NOT NULL,
    "Name" varchar(255) NOT NULL,
    "Email" varchar(255) NOT NULL,
    CONSTRAINT user_pk PRIMARY KEY ("Id")
);
