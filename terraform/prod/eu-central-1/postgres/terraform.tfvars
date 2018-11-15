terragrunt = {
  terraform {
    source = "../../../modules//postgres"
  }

  # Include all settings from the root terraform.tfvars file
  include = {
    path = "${find_in_parent_folders()}"
  }
}

name = "ts"
