-- 2019-05-28 06:37:12 : add_rootid_to_capability
ALTER TABLE public."Capability"
ADD COLUMN "RootId" VARCHAR(255);