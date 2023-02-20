targetScope = 'subscription'

param projectname string
param location string = 'westeurope'
param linuxFxVersion string = 'DOTNETCORE|7.0' // az webapp list-runtimes --os linux -o table
param runableDllName string = 'IOGameServer.dll'

resource resourcegroup 'Microsoft.Resources/resourceGroups@2022-09-01' = {
  name: projectname
  location: location
}

module appserviceplan './resources/app-service-plan.bicep' = {
  name: '${projectname}-plan'
  scope: resourcegroup
  params: {
    location: location
    appservicename: projectname
  }
}

module appservice './resources/app-service.bicep' = {
  name: projectname
  scope: resourcegroup
  params: {
    appserviceplanId: appserviceplan.outputs.appServicePlanId
    location: location
    appservicename: projectname
    linuxFxVersion: linuxFxVersion
    runableDllName: runableDllName
  }
}
