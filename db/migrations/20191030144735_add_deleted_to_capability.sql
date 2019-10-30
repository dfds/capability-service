-- 2019-10-30 14:47:35 : add deleted to capability

ALTER TABLE public."Capability"
ADD COLUMN "Deleted" timestamp;