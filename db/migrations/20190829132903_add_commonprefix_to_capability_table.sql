-- 2019-08-29 13:29:03 : Add CommonPrefix to Capability

ALTER TABLE public."Capability"
ADD COLUMN "TopicCommonPrefix" VARCHAR(255);