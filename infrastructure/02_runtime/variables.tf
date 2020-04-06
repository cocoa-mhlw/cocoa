variable "resource_group_name" {
    type = string
    default = "covid19rader-rg"
    description = "Resource Group Name for the terraform state."
}

variable "location" {
    type = string
    default = "japaneast"
    description = "Location of the resources."
}

variable "failover_location" {
    type = string
    default = "japanwest"
    description = "Location for the failover of CosmosDB"
}

variable "cosmos_db_name_prefix" {
    type = string
    default = "covid19rader-db"
    description = "Prefix name of the CosmosDB"
}

variable "cosmos_db_offer_type" {
    type = string
    default = "Standard"
    description = "CosmosDB offer type. Only Standard allowed"
}

variable "cosmos_db_kind" {
    type = string
    default = "GlobalDocumentDB" 
    description = "CosmosDB kind" 
}

variable "cosmos_db_consistency_level" {
    type = string
    default = "BoundedStaleness"
}

variable "cosmos_db_max_interval_in_seconds" {
    type = number
    default = 10
}
variable "cosmos_db_max_staleness_prefix" {
    type = number
    default = 200
}

variable "cosmos_db_database_name" {
    type = string
    default = "covid19radar"
    description = "Database name of CosmosDB" 
}

variable "cosmos_db_database_throughput" {
    type = number
    default = 400
    description = "Thoughput of Database"
}

variable "notification_hub_namespace_name" {
    type = string
    default = "covid19radar-ns"
    description = "Notification Hub Namespace Name"
}

variable "notification_hub_name" {
    type = string
    default = "covid19radar-hub"
    description = "Notification Hub Name"
} 

variable "notification_hub_sku" {
    type = string
    default = "Free"
}

variable "function_app_name_prefix" {
    type = string
    default = "covid19radar"
    description = "function app name prefix"
}