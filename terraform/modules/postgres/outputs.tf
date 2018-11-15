output "connection_string" {
  # value = "(Important, exclude :1433)  ---  host=${aws_db_instance.postgres.endpoint};port=1433;database=${var.servicename};username=api;password=${var.password};SSL Mode=Require;"
  description = "the connection string"
  value       = "host=${replace(aws_db_instance.postgres.endpoint, format(":%s", var.port), "")};port=${var.port};database=${var.name};username=${var.master_username};password=${var.master_password};SSL Mode=Require;"
}

output "db_host" {
  description = "host address of the database instance"
  value       = "${replace(aws_db_instance.postgres.endpoint, format(":%s", var.port), "")}"
}

output "db_port" {
  description = "the port of the database instance"
  value       = "${var.port}"
}

output "db_username" {
  description = "the master username of the database instance"
  value       = "${var.master_username}"
}

output "db_password" {
  description = "the master password of the database instance"
  value       = "${var.master_password}"
}
