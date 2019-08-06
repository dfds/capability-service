-- 2019-08-06 10:04:13 : add message contract table

CREATE TABLE public."MessageContract" (
    "Type" varchar(255) NOT NULL,
    "Description" text NOT NULL,
    "Content" text NOT NULL,
    "TopicId" uuid NOT NULL,
    CONSTRAINT messagecontract_pk PRIMARY KEY ("Type")
);