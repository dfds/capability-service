output "connection_string" {
  # value = "(Important, exclude :1433)  ---  host=${aws_db_instance.postgres.endpoint};port=1433;database=${var.servicename};username=api;password=${var.password};SSL Mode=Require;"
  description = "the connection string"
  value       = "host=${replace(aws_db_instance.postgres.endpoint, format(":%s", var.port), "")};port=${var.port};database=${var.name};username=${var.master_username};password=${var.master_password};SSL Mode=Require;"
}
