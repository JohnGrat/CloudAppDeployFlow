

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
  name                = "example-acsd"
  resource_group_name = local.RGname
  account_name        = azurerm_cosmosdb_account.db.name
  

  depends_on = [ azurerm_cosmosdb_account.db ]
}

resource "azurerm_cosmosdb_sql_container" "example" {
  name                  = "example-container"
  resource_group_name   = local.RGname
  account_name          = azurerm_cosmosdb_account.db.name
  database_name         = azurerm_cosmosdb_sql_database.example.name
  partition_key_path    = "/definition/id"
  partition_key_version = 1
  throughput            = 400
  

  indexing_policy {
    indexing_mode = "consistent"

    included_path {
      path = "/*"
    }

    included_path {
      path = "/included/?"
    }

    excluded_path {
      path = "/excluded/?"
    }
  }

  unique_key {
    paths = ["/definition/idlong", "/definition/idshort"]
  }

  depends_on = [ azurerm_cosmosdb_sql_database.example ]
}


output "primary_key" {
    value = nonsensitive(azurerm_cosmosdb_account.db.connection_strings[0])
}
