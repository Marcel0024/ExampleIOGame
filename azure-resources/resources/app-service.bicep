param appserviceplanId string
param location string
param appservicename string
param linuxFxVersion string
param runableDllName string

resource appservice 'Microsoft.Web/sites@2022-03-01' = {
  name: appservicename
  location: location
  properties: {
    serverFarmId: appserviceplanId
    customDomainVerificationId: 'DNS Record verification'
    enabled: true
    siteConfig: {
      linuxFxVersion: linuxFxVersion
      appCommandLine: 'dotnet ${runableDllName}'
      appSettings: [
        {
          name: 'ASPNETCORE_ENVIRONMENT'
          value: 'Production'
        }
        {
          name: 'Logging:LogLevel:Default'
          value: 'Information'
        }
      ]
      use32BitWorkerProcess: false
      webSocketsEnabled: true
      alwaysOn: true
      http20Enabled: true
      autoHealEnabled: true
      netFrameworkVersion: 'v7.0'
      ftpsState: 'Disabled'
    }
    clientAffinityEnabled: false
    httpsOnly: true
  }
}
