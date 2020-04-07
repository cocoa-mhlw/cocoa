variable "resource_group_name" {
    type = string
    default = "terraform-state-rg"
    description = "Resource Group Name for the terraform state."
}

variable "location" {
    type = string
    default = "japaneast"
    description = "Location of the resources."
}

variable "storage_account_name_prefix" {
    type = string
    default = "covid19radar"
    description = "Terraform create a Storage Account Name from this prefix. You will see the name as output variables." 
}

variable "storage_account_account_tier" {
    type = string
    default = "Standard"
}

variable "storage_account_account_replication_type" {
    type = string
    default = "LRS" # LRS, GRS, RAGRS, ZRS
}

variable "storage_account_account_kind" {
    type = string
    default = "StorageV2"
}

variable "container_name" {
    type = string
    default = "tfstate"
    description = "Container name that is created for terraform state."
}

variable "identity_type" {
    type = string
    default = "SystemAssigned"
}

variable "tags_environment" {
    type = string
    default = "production"
    description = "Tags for the environment."
}

variable "tags_version" {
    type = string
    default = "1.0.0"
    description = "Semantic version for the infrastructure."
}