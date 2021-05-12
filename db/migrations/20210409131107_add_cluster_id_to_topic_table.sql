-- 2021-04-09 13:11:07 : add cluster id to topic table

ALTER TABLE public."KafkaTopic" ADD COLUMN "KafkaClusterId" uuid NOT NULL DEFAULT '92e49432-d3d1-4e6c-b5ab-f7b7cb7c9a9b';
ALTER TABLE public."KafkaTopic" ALTER COLUMN "KafkaClusterId" DROP DEFAULT;
ALTER TABLE public."KafkaTopic" ADD UNIQUE ("Name", "KafkaClusterId");