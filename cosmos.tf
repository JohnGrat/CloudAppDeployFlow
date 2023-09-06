

resource "azurerm_cosmosdb_account" "db" {
  name                = "tfex-cosmos-db-1"
  location            = local.RGlocation
  resource_group_name = local.RGname
  offer_type          = "Standard"
  kind                = "GlobalDocumentDB"
  

  consistency_policy {
    consistency_level = "Session"
  }

  geo_location {
    location          = local.RGlocation
    failover_priority = 0
  }

  depends_on = [ azurerm_resource_group.RG-TerraformLABB2-JG ]
}


resource "azurerm_cosmosdb_sql_database" "example" {
  name                = "WeatherDb"
  resource_group_name = local.RGname
  account_name        = azurerm_cosmosdb_account.db.name
  
  
  depends_on = [ azurerm_cosmosdb_account.db ]
}

resource "azurerm_cosmosdb_sql_container" "example" {
  name                  = "Forecasts"
  resource_group_name   = local.RGname
  account_name          = azurerm_cosmosdb_account.db.name
  database_name         = azurerm_cosmosdb_sql_database.example.name
  partition_key_path    = "/partitionKey"
  partition_key_version = 1
  throughput            = 400

  depends_on = [ azurerm_cosmosdb_sql_database.example ]
}


output "primary_key" {
    value = nonsensitive(azurerm_cosmosdb_account.db.connection_strings[0])
}
