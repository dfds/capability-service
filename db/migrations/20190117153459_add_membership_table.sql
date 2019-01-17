-- 2019-01-17 15:34:59 : add membership table

CREATE TABLE public."Membership" (
    "Id" uuid NOT NULL,
    "MemberEmail" varchar(255) NOT NULL,
    "TeamId" uuid NOT NULL,
    CONSTRAINT membership_pk PRIMARY KEY ("Id")
);
