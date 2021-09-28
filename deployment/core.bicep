
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
        isZoneRedundant: true
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
            path: '/*'
          }
        ]
        excludedPaths: [
          {
            path: '/"_etag"/?'
          }
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
            path: '/*'
          }
        ]
        excludedPaths: [
          {
            path: '/"_etag"/?'
          }
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
      indexingPolicy: {
        includedPaths: [
          {
            path: '/*'
          }
        ]
        excludedPaths: [
          {
            path: '/"_etag"/?'
          }
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
      indexingPolicy: {
        includedPaths: [
          {
            path: '/*'
          }
        ]
        excludedPaths: [
          {
            path: '/"_etag"/?'
          }
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

output cosmosdbName string = cosmosdb.name

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

output keyvaultName string = keyvault.name

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

resource appConfig 'Microsoft.AppConfiguration/configurationStores@2021-03-01-preview' = {
  name: name
  location: resourceGroup().location
  sku: {
    name: 'free'
  }
  properties: {
  }
}

output appConfigName string = appConfig.name

resource appConfigDataReader 'Microsoft.Authorization/roleAssignments@2020-08-01-preview' = {
  name: guid('516239f1-63e1-4d78-a4de-a74fb236a071', principalId, keyvault.id)
  scope: keyvault
  properties: {
    principalId: principalId
    principalType: 'ServicePrincipal'
    roleDefinitionId: '/subscriptions/${subscription().subscriptionId}/providers/Microsoft.Authorization/roleDefinitions/516239f1-63e1-4d78-a4de-a74fb236a071'
  }
  dependsOn: [
    appConfig
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

output packageContainerName string = packagesContainer.name

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

resource cosmosdbListKeysRole 'Microsoft.Authorization/roleDefinitions@2018-01-01-preview' = {
  name: '5e66d809-b24f-46a8-bc5d-141bd610c09a'
  properties: {
    roleName: 'DocumentDB Keys Accessor'
    description: 'List keys of assigned DocumentDBs'
    type: 'customRole'
    permissions: [
      {
        actions: [
          'Microsoft.DocumentDB/databaseAccounts/listKeys/action'
        ]
      }
    ]
    assignableScopes:[
      '${subscription().id}/resourceGroups/${resourceGroup().name}'
    ]
  }
}

resource workspace 'Microsoft.OperationalInsights/workspaces@2021-06-01' = {
  name: name
  location: resourceGroup().location
  properties: {
    sku: {
      name: 'PerGB2018'
    }
    retentionInDays: 30
  }
}

resource datasource 'Microsoft.OperationalInsights/workspaces/dataSources@2020-08-01' = {
  name: '${name}/WindowsEventsApplication'
  kind: 'WindowsEvent'
  properties: {
    eventLogName: 'Application'
    eventTypes: [
      {
        eventType: 'Error'
      }
      {
        eventType: 'Warning'
      }
      {
        eventType: 'Information'
      }
    ]
  }
  dependsOn:[
    workspace
  ]
}

output workspaceId string = workspace.properties.customerId
output workspaceKey string = workspace.listKeys().primarySharedKey
