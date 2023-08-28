resource "azurerm_service_plan" "SP-CloudApp-JG" {
  name                = "SP-CloudApp-JG"
  resource_group_name = local.RGname
  location            = local.RGlocation
  os_type             = "Linux"
  sku_name            = "P1v2"

  depends_on = [ azurerm_resource_group.RG-TerraformLABB2-JG ]
}

resource "azurerm_linux_web_app" "App-CloudApp-JG" {
  name                = "App-CloudApp-JG"
  resource_group_name = local.RGname
  location            = local.RGlocation
  service_plan_id     = azurerm_service_plan.SP-CloudApp-JG.id

  site_config {
    application_stack {
      current_stack = "dotnet"
      dotnet_version = "v7.0"
    }
  }

  depends_on = [ azurerm_service_plan.SP-CloudApp-JG ]
}