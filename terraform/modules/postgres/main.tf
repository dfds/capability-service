provider "aws" {
  region = "${var.aws_region}"
}

terraform {
  # The configuration for this backend will be filled in by Terragrunt      
  backend "s3" {}
}

resource "aws_security_group" "onprem" {
  name        = "${var.team}-postgres10-sg"
  description = "Allow all inbound traffic"

  ingress {
    from_port   = 1433
    to_port     = 1433
    protocol    = "TCP"
    cidr_blocks = ["0.0.0.0/0"]
  }

  tags {
    team        = "${var.team}"
    environment = "${var.environment}"
  }
}

resource "aws_db_parameter_group" "dbparams" {
  name        = "${var.team}-${var.environment}-postgres10-force-ssl"
  description = "force ssl encryption for postgres10"
  family      = "postgres10"

  parameter {
    name  = "rds.force_ssl"
    value = "1"
  }

  tags {
    team        = "${var.team}"
    environment = "${var.environment}"
  }
}

resource "aws_db_instance" "postgres" {
  engine                  = "postgres"
  engine_version          = "10.4"
  publicly_accessible     = "true"
  backup_retention_period = 10
  apply_immediately       = true
  identifier              = "postgres-${var.team}-${var.environment}"
  parameter_group_name    = "${aws_db_parameter_group.dbparams.name}"
  vpc_security_group_ids  = ["${aws_security_group.onprem.id}"]

  # deletion_protection
  #   final_snapshot_identifier = "sn-${var.servicename}-${var.environment}-final"

  # configurable
  storage_type      = "${var.storage_type}"
  instance_class    = "${var.instance_class}"
  allocated_storage = "${var.allocated_storage}"
  port              = "${var.port}"
  name              = "${var.name}"
  username          = "${var.master_username}"
  password          = "${var.master_password}"
  # TODO: DO NOT COPY THIS SETTING INTO YOUR PRODUCTION DBS. It's only here to make testing this code easier!
  skip_final_snapshot = true
  tags {
    team        = "${var.team}"
    environment = "${var.environment}"
  }
}
