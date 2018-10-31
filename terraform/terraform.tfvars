terragrunt = {
  # Configure Terragrunt to automatically store tfstate files in an S3 bucket
  remote_state {
    backend = "s3"

    config {
      encrypt        = true
      region         = "eu-central-1"
      bucket         = "dfds-ded-terraform-state"
      key            = "${path_relative_to_include()}/terraform.tfstate"
      dynamodb_table = "terraform-locks"

      s3_bucket_tags {
        owner = "ded"
        name  = "Terraform state storage"
      }

      dynamodb_table_tags {
        owner = "ded"
        name  = "Terraform lock table"
      }
    }
  }

  # Configure Terragrunt to use common var files to help you keep often-repeated variables (e.g., account ID) DRY.
  # Note that even though Terraform automatically pulls in terraform.tfvars, we include it explicitly at the end of the
  # list to make sure its variables override anything in the common var files.
  terraform {
    extra_arguments "common_vars" {
      commands = ["${get_terraform_commands_that_need_vars()}"]

      optional_var_files = [
        "${get_tfvars_dir()}/${find_in_parent_folders("account.tfvars", "skip-account-if-does-not-exist")}",
        "${get_tfvars_dir()}/${find_in_parent_folders("region.tfvars", "skip-region-if-does-not-exist")}",
        "${get_tfvars_dir()}/${find_in_parent_folders("env.tfvars", "skip-env-if-does-not-exist")}",
        "${get_tfvars_dir()}/terraform.tfvars",
      ]
    }
  }
}
