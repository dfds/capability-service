-- 2021-04-09 13:11:07 : add cluster id to topic table

ALTER TABLE public."KafkaTopic" ADD COLUMN "KafkaClusterId" uuid NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
ALTER TABLE public."KafkaTopic" ALTER COLUMN "KafkaClusterId" DROP DEFAULT;
