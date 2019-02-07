-- 2019-02-06 15:54:55 : rename team table to capability

ALTER TABLE public."Team"
RENAME TO "Capability";

ALTER TABLE public."Membership"
RENAME "TeamId" TO "CapabilityId";