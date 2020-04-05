-- 2020-04-03 15:00:52 : add_creation_timestamp_and_last_modified_timestamp_to_topic_table

ALTER TABLE public."KafkaTopic"
ADD COLUMN "Created" timestamp;

ALTER TABLE public."KafkaTopic"
ADD COLUMN "LastModified" timestamp;