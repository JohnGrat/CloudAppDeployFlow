resource "azurerm_virtual_network" "MyAzureApp" {
  name                = "app-network"
  address_space       = ["10.0.0.0/16"]
  location            = local.RGlocation
  resource_group_name = local.RGname
  depends_on = [ azurerm_resource_group.RG-TerraformLABB2-JG ]
}

resource "azurerm_subnet" "WebAppsSubnet" {
  name                 = "WebApps-subnet"
  resource_group_name  = local.RGname
  virtual_network_name = azurerm_virtual_network.MyAzureApp.name
  address_prefixes     = ["10.0.1.0/24"]

  delegation {
    name = "example-delegation"

    service_delegation {
      name    = "Microsoft.Web/serverFarms"
      actions = ["Microsoft.Network/virtualNetworks/subnets/action"]
    }
  }

  depends_on = [ azurerm_virtual_network.MyAzureApp ]
}


resource "azurerm_app_service_virtual_network_swift_connection" "Production_Subnet" {
  
  app_service_id = azurerm_windows_web_app.CloudAppDeployWA.id
  subnet_id      = azurerm_subnet.WebAppsSubnet.id
  depends_on = [ azurerm_subnet.WebAppsSubnet , azurerm_windows_web_app.CloudAppDeployWA ]
}