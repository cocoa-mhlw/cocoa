resource "azurerm_resource_group" "tfstate" {
  name     = var.resource_group_name
  location = var.location
}

resource "random_string" "storage_random" {
  length = 3 
  special = false
  lower = true
  min_lower = 3
  
  keepers = {
    rg = "${azurerm_resource_group.tfstate.id}"
  }
}

resource "azurerm_storage_account" "tfstate" {
  name                     = "${var.storage_account_name_prefix}${random_string.storage_random.result}" # globally unique
  resource_group_name      = azurerm_resource_group.tfstate.name
  location                 = azurerm_resource_group.tfstate.location
  account_tier             = var.storage_account_account_tier
  account_replication_type = var.storage_account_account_replication_type 
  account_kind             = var.storage_account_account_kind

  identity {
    type = var.identity_type
  }

  tags = {
    environment = var.tags_environment
    version     = var.tags_version
  }
    depends_on = [azurerm_resource_group.tfstate]
}

resource "azurerm_storage_container" "tfstate" {
  name = var.container_name
  storage_account_name  = azurerm_storage_account.tfstate.name
  container_access_type = "private"
  depends_on = [azurerm_storage_account.tfstate]
}

resource "local_file" "tfstate" {
    content = "{\"stroage_account_name\":\"${azurerm_storage_account.tfstate.name}\"}"
    filename = "storage_account_name.json"
}

