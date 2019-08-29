-- 2019-08-29 13:31:09 : added prefixes to topic table

ALTER TABLE public."Topic"
ADD COLUMN "NameBusinessArea" VARCHAR(255);

ALTER TABLE public."Topic"
ADD COLUMN "NameType" VARCHAR(255);

ALTER TABLE public."Topic"
ADD COLUMN "NameMisc" VARCHAR(255);