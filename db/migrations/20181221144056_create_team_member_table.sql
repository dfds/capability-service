-- 2018-12-21 14:40:56 : create team member table

CREATE TABLE public."Member" (
    "Email" varchar(255) NOT NULL,
    "TeamId" uuid NOT NULL
);
