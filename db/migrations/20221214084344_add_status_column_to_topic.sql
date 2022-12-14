-- 2022-12-14 08:43:44 : add status column to topic

ALTER TABLE public."KafkaTopic"
ADD COLUMN "Status" varchar(50) DEFAULT 'Provisioned';
