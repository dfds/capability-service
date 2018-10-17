-- 2018-10-17 19:43:44 : create membership table

CREATE TABLE public."Membership" (
    "Id" uuid NOT NULL,
    "StartedDate" timestamp NOT NULL,
    "UserId" varchar(255) NOT NULL,
    "TeamId" uuid NOT NULL,
    "Type" varchar(255) NOT NULL,
    CONSTRAINT membership_pk PRIMARY KEY ("Id"),
    CONSTRAINT membership_team_fk FOREIGN KEY ("TeamId") REFERENCES "Team"("Id"),
    CONSTRAINT membership_user_fk FOREIGN KEY ("UserId") REFERENCES "User"("Id")
);
