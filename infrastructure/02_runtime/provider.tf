provider "azurerm" {
  version = "2.4.0"
  features {}
}

terraform {
  required_version = ">= v0.12.18"
  backend "azurerm" {
    resource_group_name = "terraform-state-rg" # Resource Group Name for the Storage Account. 
    container_name = "tfstate"
    key = "prod.terraform.tfstate"
  }
}