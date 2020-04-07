# Create a resource group
resource "azurerm_resource_group" "prod" {
  name     = var.resource_group_name
  location = var.location
}

# Create a cosmos db account and create a database

resource "random_string" "prod" {
  length = 3 
  special = false
  lower = true
  min_lower = 3
  
  keepers = {
    rg = "${azurerm_resource_group.prod.id}"
  }
} 
resource "azurerm_cosmosdb_account" "prod" {
  name                = "${var.cosmos_db_name_prefix}-${random_string.prod.result}"
  location            = azurerm_resource_group.prod.location
  resource_group_name = azurerm_resource_group.prod.name
  offer_type          = var.cosmos_db_offer_type
  kind                = var.cosmos_db_kind

  # Enable this line if you want Geo Location.
  # enable_automatic_failover = true

  consistency_policy {
    consistency_level       = var.cosmos_db_consistency_level
    max_interval_in_seconds = var.cosmos_db_max_interval_in_seconds
    max_staleness_prefix    = var.cosmos_db_max_staleness_prefix
  }

 # Enable this if you want Geo Location.
 # geo_location {
 #   location          = var.failover_location
 #   failover_priority = 1
 # }

  geo_location {
    location          = azurerm_resource_group.prod.location
    failover_priority = 0 # Should be one if you want to enable this.
  }
    depends_on = [azurerm_resource_group.prod]
}

resource "azurerm_cosmosdb_sql_database" "prod" {
  name                = var.cosmos_db_database_name
  resource_group_name = azurerm_cosmosdb_account.prod.resource_group_name
  account_name        = azurerm_cosmosdb_account.prod.name
  throughput          = var.cosmos_db_database_throughput
  depends_on = [azurerm_cosmosdb_account.prod]
}

# Create a Notification Hub

resource "azurerm_notification_hub_namespace" "prod" {
  name                = var.notification_hub_namespace_name
  resource_group_name = azurerm_resource_group.prod.name
  location            = azurerm_resource_group.prod.location
  namespace_type      = "NotificationHub"

  sku_name = var.notification_hub_sku
}

resource "azurerm_notification_hub" "prod" {
  name                = var.notification_hub_name
  namespace_name      = azurerm_notification_hub_namespace.prod.name
  resource_group_name = azurerm_resource_group.prod.name
  location            = azurerm_resource_group.prod.location
}

resource "azurerm_notification_hub_authorization_rule" "prod" {
  name                  = "management-auth-rule"
  notification_hub_name = azurerm_notification_hub.prod.name
  namespace_name        = azurerm_notification_hub_namespace.prod.name
  resource_group_name   = azurerm_resource_group.prod.name
  manage                = true
  send                  = true
  listen                = true
}

# Create a FunctionApp
resource "random_string" "sa" {
  length  = 8
  lower = true
  upper = false
  special = false
}

resource "azurerm_storage_account" "prod" {
  name                     = "${var.function_app_name_prefix}${random_string.sa.result}"
  resource_group_name      = azurerm_resource_group.prod.name
  location                 = azurerm_resource_group.prod.location
  account_tier             = "Standard"
  account_replication_type = "LRS"
  depends_on               = [azurerm_resource_group.prod]
}

resource "azurerm_app_service_plan" "prod" {
  name                = "${var.function_app_name_prefix}plan"
  location            =  azurerm_resource_group.prod.location
  resource_group_name = azurerm_resource_group.prod.name
  kind                = "FunctionApp"

  sku {
    tier = "Dynamic"
    size = "Y1"
  }

  depends_on = [azurerm_resource_group.prod]
}

resource "azurerm_application_insights" "prod" {
  name                = "${var.function_app_name_prefix}-appinsights"
  location            = "eastus"
  resource_group_name = azurerm_resource_group.prod.name
  application_type    = "web"
}

resource "azurerm_function_app" "prod" {
  name                      = "${var.function_app_name_prefix}${random_string.prod.result}"
  location                  = azurerm_resource_group.prod.location
  resource_group_name       = azurerm_resource_group.prod.name
  app_service_plan_id       = azurerm_app_service_plan.prod.id
  storage_connection_string = azurerm_storage_account.prod.primary_connection_string
  version                   = "~3"

  app_settings = {
    FUNCTIONS_WORKER_RUNTIME = "dotnet"
    APPINSIGHTS_INSTRUMENTATIONKEY = azurerm_application_insights.prod.instrumentation_key
    COSMOS_DB_MASTER_KEY = azurerm_cosmosdb_account.prod.primary_readonly_master_key
    NOTIFICATION_HUB_PRIMARY_KEY = azurerm_notification_hub_authorization_rule.prod.primary_access_key
  }

  depends_on = [azurerm_storage_account.prod, azurerm_app_service_plan.prod, azurerm_application_insights.prod, azurerm_cosmosdb_account.prod, azurerm_notification_hub_authorization_rule.prod]
}