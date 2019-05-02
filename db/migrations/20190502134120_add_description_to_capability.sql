-- 2019-05-02 13:41:20 : add_description_to_capability
ALTER TABLE public."Capability"
ADD COLUMN "Description" VARCHAR(255);