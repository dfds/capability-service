-- 2020-04-15 11:11:57 : add_configuration_to_topic_table

ALTER TABLE public."KafkaTopic"
ADD COLUMN "Configurations" varchar(4096);