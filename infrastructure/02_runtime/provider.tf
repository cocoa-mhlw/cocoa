# This Source Code Form is subject to the terms of the Mozilla Public
# License, v. 2.0. If a copy of the MPL was not distributed with this
# file, You can obtain one at https://mozilla.org/MPL/2.0/.

provider "azurerm" {
  version = "2.4.0"
  features {}
}

terraform {
  required_version = ">= v0.12.18"
  backend "azurerm" {
    resource_group_name = "terraform-state-rg" # Resource Group Name for the Storage Account. 
    container_name      = "tfstate"
    key                 = "prod.terraform.tfstate"
  }
}