-- 2019-05-24 15:11:31 : add_aws_context_fields
ALTER TABLE public."Context"
ADD COLUMN "AWSAccountId" VARCHAR(12),
ADD COLUMN "AWSRoleArn" VARCHAR(255),
ADD COLUMN "AWSRoleEmail" VARCHAR(255);