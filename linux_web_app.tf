resource "azurerm_service_plan" "ASPcloudAppDeploy" {
  name                = "ASPcloudAppDeploy"
  resource_group_name = local.RGname
  location            = local.RGlocation
  sku_name            = "B1"
  os_type             = "Linux"
  depends_on = [ azurerm_resource_group.RG-TerraformLABB2-JG ]
}

resource "azurerm_linux_web_app" "CloudAppDeployWA" {
  name                = "cloudAppDeploywa2"
  resource_group_name = local.RGname
  location            = local.RGlocation
  service_plan_id     = azurerm_service_plan.ASPcloudAppDeploy.id

 

  site_config {
    application_stack {
      dotnet_version = "7.0"
    }
  }

  connection_string {
    name  = "Database"
    type  = "Custom"
    value = azurerm_cosmosdb_account.db.connection_strings[0]
  }
  depends_on = [ azurerm_service_plan.ASPcloudAppDeploy, azurerm_cosmosdb_account.db ]
}


resource "azurerm_source_control_token" "token" {
  type  = "GitHub"
  token = var.GitHubToken
  depends_on = [ azurerm_resource_group.RG-TerraformLABB2-JG ]
}

resource "azurerm_app_service_source_control" "production_code" {
  app_id   = azurerm_linux_web_app.CloudAppDeployWA.id
  repo_url = "https://github.com/JohnGrat/CloudAppDeployFlow"
  branch   = "main"

  depends_on = [ azurerm_linux_web_app.CloudAppDeployWA ,
  azurerm_source_control_token.token]
}