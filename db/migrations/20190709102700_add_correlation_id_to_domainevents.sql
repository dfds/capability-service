-- 2019-05-28 06:37:12 : add_rootid_to_capability
ALTER TABLE public."DomainEvent"
ADD COLUMN "CorrelationId" VARCHAR(255);