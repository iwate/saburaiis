param principalId string
param cosmosdbName string
param keyvaultName string
param appConfigName string
param packageContainerName string

resource keyvault 'Microsoft.KeyVault/vaults@2021-04-01-preview' existing = {
  name: keyvaultName
}

resource keyvaultReader 'Microsoft.Authorization/roleAssignments@2020-08-01-preview' = {
  name: guid('21090545-7ca7-4776-b22c-e363652d74d2', principalId, keyvault.id)
  scope: keyvault
  properties: {
    principalId: principalId
    principalType: 'ServicePrincipal'
    roleDefinitionId: '/subscriptions/${subscription().subscriptionId}/providers/Microsoft.Authorization/roleDefinitions/21090545-7ca7-4776-b22c-e363652d74d2'
  }
  dependsOn: [
    keyvault
  ]
}

resource keyvaultSecretsUser 'Microsoft.Authorization/roleAssignments@2020-08-01-preview' = {
  name: guid('4633458b-17de-408a-b874-0445c86b69e6', principalId, keyvault.id)
  scope: keyvault
  properties: {
    principalId: principalId
    principalType: 'ServicePrincipal'
    roleDefinitionId: '/subscriptions/${subscription().subscriptionId}/providers/Microsoft.Authorization/roleDefinitions/4633458b-17de-408a-b874-0445c86b69e6'
  }
  dependsOn: [
    keyvault
  ]
}

resource packagesContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2021-04-01' existing = {
  name: packageContainerName
}

resource storageBlobDataContributor 'Microsoft.Authorization/roleAssignments@2020-08-01-preview' = {
  name: guid('ba92f5b4-2d11-453d-a403-e96b0029c9fe', principalId, packagesContainer.id)
  scope: packagesContainer
  properties: {
    principalId: principalId
    principalType: 'ServicePrincipal'
    roleDefinitionId: '/subscriptions/${subscription().subscriptionId}/providers/Microsoft.Authorization/roleDefinitions/ba92f5b4-2d11-453d-a403-e96b0029c9fe'
  }
  dependsOn: [
    packagesContainer
  ]
}

resource cosmosdb 'Microsoft.DocumentDB/databaseAccounts@2021-06-15' existing = {
  name: cosmosdbName
}

resource cosmosdbDataContributor 'Microsoft.DocumentDB/databaseAccounts/sqlRoleAssignments@2021-06-15' = {
  name: '${cosmosdbName}/${guid('00000000-0000-0000-0000-000000000002', principalId, cosmosdb.id)}'
  properties: {
    scope: cosmosdb.id
    principalId: principalId
    roleDefinitionId: '/subscriptions/${subscription().subscriptionId}/resourceGroups/${resourceGroup().name}/providers/Microsoft.DocumentDB/databaseAccounts/${cosmosdbName}/sqlRoleDefinitions/00000000-0000-0000-0000-000000000002'
  }
  dependsOn:[
    cosmosdb
  ]
}

resource cosmosdbListKeysReader 'Microsoft.Authorization/roleAssignments@2020-08-01-preview' = {
  name: guid('5e66d809-b24f-46a8-bc5d-141bd610c09a', principalId, cosmosdb.id)
  scope: cosmosdb
  properties: {
    principalId: principalId
    principalType: 'ServicePrincipal'
    roleDefinitionId: '/subscriptions/${subscription().subscriptionId}/providers/Microsoft.Authorization/roleDefinitions/5e66d809-b24f-46a8-bc5d-141bd610c09a'
  }
  dependsOn:[
    cosmosdb
  ]
}


resource appConfig 'Microsoft.AppConfiguration/configurationStores@2021-03-01-preview' existing = {
  name: appConfigName
}

resource appConfigDataReader 'Microsoft.Authorization/roleAssignments@2020-08-01-preview' = {
  name: guid('516239f1-63e1-4d78-a4de-a74fb236a071', principalId, keyvault.id)
  scope: appConfig
  properties: {
    principalId: principalId
    principalType: 'ServicePrincipal'
    roleDefinitionId: '/subscriptions/${subscription().subscriptionId}/providers/Microsoft.Authorization/roleDefinitions/516239f1-63e1-4d78-a4de-a74fb236a071'
  }
  dependsOn: [
    appConfig
  ]
}
