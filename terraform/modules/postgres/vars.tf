variable "aws_region" {
  description = "The AWS region to deploy to (e.g. us-east-1)"
}

variable "storage_type" {
  description = "The type of storage to use for the DB. Must be one of: standard, gp2, or io1."
  default     = "gp2"
}

variable "instance_class" {
  description = "The instance class of the DB (e.g. db.t2.micro)"
  default     = "db.t2.micro"
}

variable "allocated_storage" {
  description = "The amount of space, in GB, to allocate for the DB"
  default     = 20
}

variable "port" {
  default     = 1433
  description = "the port to use for the database"
}

variable "name" {
  description = "The name of the DB"
}

variable "master_username" {
  description = "The username for the master user of the DB"
}

variable "master_password" {
  description = "The password for the master user of the DB"
}

variable "team" {
  description = "the name of the team"
}

variable "environment" {
  default     = "dev"
  description = "the environment"
}
