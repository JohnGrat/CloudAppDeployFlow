resource "azurerm_service_plan" "ASPcloudAppDeploy" {
  name                = "ASPcloudAppDeploy"
  resource_group_name = local.RGname
  location            = local.RGlocation
  sku_name            = "P1v3"
  os_type             = "Windows"
  depends_on = [ azurerm_resource_group.RG-TerraformLABB2-JG ]
}

resource "azurerm_windows_web_app" "CloudAppDeployWA" {
  name                = "cloudAppDeploywa2"
  resource_group_name = local.RGname
  location            = local.RGlocation
  service_plan_id     = azurerm_service_plan.ASPcloudAppDeploy.id

  site_config {
    application_stack {
      current_stack = "dotnet"
      dotnet_version = "v7.0"
    }
  } 
  

  depends_on = [ azurerm_service_plan.ASPcloudAppDeploy ]
}

resource "azurerm_windows_web_app_slot" "Production" {
  name           = "production-slot"
  app_service_id = azurerm_windows_web_app.CloudAppDeployWA.id

  site_config {
    application_stack {
      current_stack = "dotnet"
      dotnet_version = "v7.0"
    }
  }

  depends_on = [ azurerm_windows_web_app.CloudAppDeployWA ]
}


resource "azurerm_source_control_token" "token" {
  type  = "GitHub"
  token = var.GitHubToken
  depends_on = [ azurerm_resource_group.RG-TerraformLABB2-JG ]
}

resource "azurerm_app_service_source_control" "production_code" {
  app_id   = azurerm_windows_web_app.CloudAppDeployWA.id
  repo_url = "https://github.com/JohnGrat/CloudAppDeployFlow"
  branch   = "main"

  depends_on = [ azurerm_windows_web_app.CloudAppDeployWA ,
  azurerm_source_control_token.token]
}