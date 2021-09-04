
param name string

param principalId string

param cosmosdbEnableFreeTier bool = true

resource cosmosdb 'Microsoft.DocumentDB/databaseAccounts@2021-06-15' = {
  name: name
  kind: 'GlobalDocumentDB'
  location: resourceGroup().location
  properties: {
    enableFreeTier: cosmosdbEnableFreeTier
    databaseAccountOfferType: 'Standard'
    locations: [
      {
        locationName: resourceGroup().location
      }
    ]
  }
}

resource database 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2021-06-15' = {
  name: '${cosmosdb.name}/saburaiis'
  location: resourceGroup().location
  properties: {
    resource: {
      id: 'saburaiis'
    }
  }
  dependsOn: [
    cosmosdb
  ]
}

resource containerPartitions 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2021-06-15' = {
  name: '${cosmosdb.name}/saburaiis/partitions'
  location: resourceGroup().location
  properties: {
    resource: {
      id: 'partitions'
      partitionKey: {
        paths: [
          '/Name'
        ]
      }
      indexingPolicy: {
        includedPaths: [
          {
            path: '/'
          }
        ]
        excludedPaths: [
          {
            path: '/ApplicationPools/*'
          }
          {
            path: '/Sites/*'
          }
        ]
      }
    }
  }
  dependsOn:[
    database
  ]
}

resource containerSnapshots 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2021-06-15' = {
  name: '${cosmosdb.name}/saburaiis/snapshots'
  location: resourceGroup().location
  properties: {
    resource: {
      id: 'snapshots'
      partitionKey: {
        paths: [
          '/Name'
        ]
      }
      indexingPolicy: {
        includedPaths: [
          {
            path: '/'
          }
        ]
        excludedPaths: [
          {
            path: '/ApplicationPools/*'
          }
          {
            path: '/Sites/*'
          }
        ]
      }
    }
  }
  dependsOn:[
    database
  ]
}

resource containerPackages 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2021-06-15' = {
  name: '${cosmosdb.name}/saburaiis/packages'
  location: resourceGroup().location
  properties: {
    resource: {
      id: 'packages'
      partitionKey: {
        paths: [
          '/Name'
        ]
      }
    }
  }
  dependsOn:[
    database
  ]
}

resource containerInstances 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2021-06-15' = {
  name: '${cosmosdb.name}/saburaiis/instances'
  location: resourceGroup().location
  properties: {
    resource: {
      id: 'instances'
      partitionKey: {
        paths: [
          '/ScaleSetName'
        ]
      }
    }
  }
  dependsOn:[
    database
  ]
}

resource cosmosdbDataContributor 'Microsoft.DocumentDB/databaseAccounts/sqlRoleAssignments@2021-06-15' = {
  name: '${cosmosdb.name}/${guid('00000000-0000-0000-0000-000000000002', principalId, cosmosdb.id)}'
  properties: {
    scope: cosmosdb.id
    principalId: principalId
    roleDefinitionId: '/subscriptions/${subscription().subscriptionId}/resourceGroups/${resourceGroup().name}/providers/Microsoft.DocumentDB/databaseAccounts/${cosmosdb.name}/sqlRoleDefinitions/00000000-0000-0000-0000-000000000002'
  }
  dependsOn: [
    cosmosdb
  ]
}

resource keyvault 'Microsoft.KeyVault/vaults@2021-04-01-preview' = {
  name: name
  location: resourceGroup().location
  properties: {
    sku: {
      family: 'A'
      name: 'standard'
    }
    tenantId: subscription().tenantId
    accessPolicies: []
    enabledForDeployment: true
    enabledForDiskEncryption: false
    enabledForTemplateDeployment: false
    enableSoftDelete: true
    softDeleteRetentionInDays: 90
    enableRbacAuthorization: true
  }
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

resource storage 'Microsoft.Storage/storageAccounts@2021-04-01' = {
  name: name
  location: resourceGroup().location
  kind: 'StorageV2'
  sku: {
    name: 'Standard_ZRS'
  }
}

resource packagesContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2021-04-01' = {
  name: '${storage.name}/default/packages'
  dependsOn:[
    storage
  ]
}

resource storageBlobDataContributor 'Microsoft.Authorization/roleAssignments@2020-08-01-preview' = {
  name: guid('ba92f5b4-2d11-453d-a403-e96b0029c9fe', principalId, keyvault.id)
  scope: keyvault
  properties: {
    principalId: principalId
    principalType: 'ServicePrincipal'
    roleDefinitionId: '/subscriptions/${subscription().subscriptionId}/providers/Microsoft.Authorization/roleDefinitions/ba92f5b4-2d11-453d-a403-e96b0029c9fe'
  }
  dependsOn: [
    storage
  ]
}
