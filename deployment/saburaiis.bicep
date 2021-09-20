targetScope = 'subscription'

param principalId string

param coreName string

param partitionName string

param vmPrefix string

param vmSku string

param vmAdminUsername string

param vmAdminPassword string

param cosmosdbEnableFreeTier bool = true

param location string = deployment().location

var vnetName = '${coreName}-vnet-${location}'

resource coreRG 'Microsoft.Resources/resourceGroups@2021-04-01' = {
  name: coreName
  location: location
}

module core './core.bicep' = {
  name: 'coreDeploy'
  scope: coreRG
  params: {
    name: coreName
    principalId: principalId
    cosmosdbEnableFreeTier: cosmosdbEnableFreeTier
  }
}

resource networkRG 'Microsoft.Resources/resourceGroups@2021-04-01' = {
  name: '${coreName}-network'
  location: location
}

module network 'network.bicep' = {
  name: 'networkDeploy'
  scope: networkRG
  params: {
    vnetName: vnetName
    subnetName: partitionName
  }
}

resource partitionRG 'Microsoft.Resources/resourceGroups@2021-04-01' = {
  name: partitionName
  location: location
}

module vmss 'partition.bicep' = {
  name: 'partitionDeploy'
  scope: partitionRG
  params: {
    name: partitionName
    vnetRGName: networkRG.name
    vnetName: vnetName
    subnetName: partitionName
    vmPrefix: vmPrefix
    vmSku: vmSku
    adminUsername: vmAdminUsername
    adminPassword: vmAdminPassword
    instanceCount: 2
    saburaiisCoreName: coreName
    workspaceId: core.outputs.workspaceId
    workspaceKey: core.outputs.workspaceKey
  }
  dependsOn:[
    core
  ]
}

module iam 'iam.bicep' = {
  name: 'assignRolesDeploy'
  scope: coreRG
  params: {
    principalId: vmss.outputs.principalId
    cosmosdbName: core.outputs.cosmosdbName
    keyvaultName: core.outputs.keyvaultName
    packageContainerName: core.outputs.packageContainerName
  }
  dependsOn: [
    core
    vmss
  ]
}

