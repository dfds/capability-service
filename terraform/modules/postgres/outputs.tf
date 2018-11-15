output "PGHOST" {
  description = "host address of the database instance"
  value       = "${replace(aws_db_instance.postgres.endpoint, format(":%s", var.port), "")}"
}
