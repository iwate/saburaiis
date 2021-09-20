param name string

param vmPrefix string

@allowed([
  'Standard_A0'
  'Standard_A1'
  'Standard_A1_v2'
  'Standard_A2'
  'Standard_A2_v2'
  'Standard_A2m_v2'
  'Standard_A3'
  'Standard_A4'
  'Standard_A4_v2'
  'Standard_A4m_v2'
  'Standard_A5'
  'Standard_A6'
  'Standard_A7'
  'Standard_A8_v2'
  'Standard_A8m_v2'
  'Standard_B12ms'
  'Standard_B16ms'
  'Standard_B1ls'
  'Standard_B1ms'
  'Standard_B1s'
  'Standard_B20ms'
  'Standard_B2ms'
  'Standard_B2s'
  'Standard_B4ms'
  'Standard_B8ms'
  'Standard_D1'
  'Standard_D1_v2'
  'Standard_D11'
  'Standard_D11_v2'
  'Standard_D11_v2_Promo'
  'Standard_D12'
  'Standard_D12_v2'
  'Standard_D12_v2_Promo'
  'Standard_D13'
  'Standard_D13_v2'
  'Standard_D13_v2_Promo'
  'Standard_D14'
  'Standard_D14_v2'
  'Standard_D14_v2_Promo'
  'Standard_D15_v2'
  'Standard_D16_v3'
  'Standard_D16_v4'
  'Standard_D16a_v4'
  'Standard_D16as_v4'
  'Standard_D16d_v4'
  'Standard_D16ds_v4'
  'Standard_D16s_v3'
  'Standard_D16s_v4'
  'Standard_D2'
  'Standard_D2_v2'
  'Standard_D2_v2_Promo'
  'Standard_D2_v3'
  'Standard_D2_v4'
  'Standard_D2a_v4'
  'Standard_D2as_v4'
  'Standard_D2d_v4'
  'Standard_D2ds_v4'
  'Standard_D2s_v3'
  'Standard_D2s_v4'
  'Standard_D3'
  'Standard_D3_v2'
  'Standard_D3_v2_Promo'
  'Standard_D32_v3'
  'Standard_D32_v4'
  'Standard_D32a_v4'
  'Standard_D32as_v4'
  'Standard_D32d_v4'
  'Standard_D32ds_v4'
  'Standard_D32s_v3'
  'Standard_D32s_v4'
  'Standard_D4'
  'Standard_D4_v2'
  'Standard_D4_v2_Promo'
  'Standard_D4_v3'
  'Standard_D4_v4'
  'Standard_D48_v3'
  'Standard_D48_v4'
  'Standard_D48a_v4'
  'Standard_D48as_v4'
  'Standard_D48d_v4'
  'Standard_D48ds_v4'
  'Standard_D48s_v3'
  'Standard_D48s_v4'
  'Standard_D4a_v4'
  'Standard_D4as_v4'
  'Standard_D4d_v4'
  'Standard_D4ds_v4'
  'Standard_D4s_v3'
  'Standard_D4s_v4'
  'Standard_D5_v2'
  'Standard_D5_v2_Promo'
  'Standard_D64_v3'
  'Standard_D64_v4'
  'Standard_D64a_v4'
  'Standard_D64as_v4'
  'Standard_D64d_v4'
  'Standard_D64ds_v4'
  'Standard_D64s_v3'
  'Standard_D64s_v4'
  'Standard_D8_v3'
  'Standard_D8_v4'
  'Standard_D8a_v4'
  'Standard_D8as_v4'
  'Standard_D8d_v4'
  'Standard_D8ds_v4'
  'Standard_D8s_v3'
  'Standard_D8s_v4'
  'Standard_D96a_v4'
  'Standard_D96as_v4'
  'Standard_DC1s_v2'
  'Standard_DC2s'
  'Standard_DC2s_v2'
  'Standard_DC4s'
  'Standard_DC4s_v2'
  'Standard_DC8_v2'
  'Standard_DS1'
  'Standard_DS1_v2'
  'Standard_DS11'
  'Standard_DS11_v2'
  'Standard_DS11_v2_Promo'
  'Standard_DS11-1_v2'
  'Standard_DS12'
  'Standard_DS12_v2'
  'Standard_DS12_v2_Promo'
  'Standard_DS12-1_v2'
  'Standard_DS12-2_v2'
  'Standard_DS13'
  'Standard_DS13_v2'
  'Standard_DS13_v2_Promo'
  'Standard_DS13-2_v2'
  'Standard_DS13-4_v2'
  'Standard_DS14'
  'Standard_DS14_v2'
  'Standard_DS14_v2_Promo'
  'Standard_DS14-4_v2'
  'Standard_DS14-8_v2'
  'Standard_DS15_v2'
  'Standard_DS2'
  'Standard_DS2_v2'
  'Standard_DS2_v2_Promo'
  'Standard_DS3'
  'Standard_DS3_v2'
  'Standard_DS3_v2_Promo'
  'Standard_DS4'
  'Standard_DS4_v2'
  'Standard_DS4_v2_Promo'
  'Standard_DS5_v2'
  'Standard_DS5_v2_Promo'
  'Standard_E16_v3'
  'Standard_E16_v4'
  'Standard_E16-4as_v4'
  'Standard_E16-4ds_v4'
  'Standard_E16-4s_v3'
  'Standard_E16-4s_v4'
  'Standard_E16-8as_v4'
  'Standard_E16-8ds_v4'
  'Standard_E16-8s_v3'
  'Standard_E16-8s_v4'
  'Standard_E16a_v4'
  'Standard_E16as_v4'
  'Standard_E16d_v4'
  'Standard_E16ds_v4'
  'Standard_E16s_v3'
  'Standard_E16s_v4'
  'Standard_E2_v3'
  'Standard_E2_v4'
  'Standard_E20_v3'
  'Standard_E20_v4'
  'Standard_E20a_v4'
  'Standard_E20as_v4'
  'Standard_E20d_v4'
  'Standard_E20ds_v4'
  'Standard_E20s_v3'
  'Standard_E20s_v4'
  'Standard_E2a_v4'
  'Standard_E2as_v4'
  'Standard_E2d_v4'
  'Standard_E2ds_v4'
  'Standard_E2s_v3'
  'Standard_E2s_v4'
  'Standard_E32_v3'
  'Standard_E32_v4'
  'Standard_E32-16as_v4'
  'Standard_E32-16ds_v4'
  'Standard_E32-16s_v3'
  'Standard_E32-16s_v4'
  'Standard_E32-8as_v4'
  'Standard_E32-8ds_v4'
  'Standard_E32-8s_v3'
  'Standard_E32-8s_v4'
  'Standard_E32a_v4'
  'Standard_E32as_v4'
  'Standard_E32d_v4'
  'Standard_E32ds_v4'
  'Standard_E32s_v3'
  'Standard_E32s_v4'
  'Standard_E4_v3'
  'Standard_E4_v4'
  'Standard_E4-2as_v4'
  'Standard_E4-2ds_v4'
  'Standard_E4-2s_v3'
  'Standard_E4-2s_v4'
  'Standard_E48_v3'
  'Standard_E48_v4'
  'Standard_E48a_v4'
  'Standard_E48as_v4'
  'Standard_E48d_v4'
  'Standard_E48ds_v4'
  'Standard_E48s_v3'
  'Standard_E48s_v4'
  'Standard_E4a_v4'
  'Standard_E4as_v4'
  'Standard_E4d_v4'
  'Standard_E4ds_v4'
  'Standard_E4s_v3'
  'Standard_E4s_v4'
  'Standard_E64_v3'
  'Standard_E64_v4'
  'Standard_E64-16as_v4'
  'Standard_E64-16ds_v4'
  'Standard_E64-16s_v3'
  'Standard_E64-16s_v4'
  'Standard_E64-32as_v4'
  'Standard_E64-32ds_v4'
  'Standard_E64-32s_v3'
  'Standard_E64-32s_v4'
  'Standard_E64a_v4'
  'Standard_E64as_v4'
  'Standard_E64d_v4'
  'Standard_E64ds_v4'
  'Standard_E64i_v3'
  'Standard_E64is_v3'
  'Standard_E64s_v3'
  'Standard_E64s_v4'
  'Standard_E8_v3'
  'Standard_E8_v4'
  'Standard_E80ids_v4'
  'Standard_E80is_v4'
  'Standard_E8-2as_v4'
  'Standard_E8-2ds_v4'
  'Standard_E8-2s_v3'
  'Standard_E8-2s_v4'
  'Standard_E8-4as_v4'
  'Standard_E8-4ds_v4'
  'Standard_E8-4s_v3'
  'Standard_E8-4s_v4'
  'Standard_E8a_v4'
  'Standard_E8as_v4'
  'Standard_E8d_v4'
  'Standard_E8ds_v4'
  'Standard_E8s_v3'
  'Standard_E8s_v4'
  'Standard_E96-24as_v4'
  'Standard_E96-48as_v4'
  'Standard_E96a_v4'
  'Standard_E96as_v4'
  'Standard_F1'
  'Standard_F16'
  'Standard_F16s'
  'Standard_F16s_v2'
  'Standard_F1s'
  'Standard_F2'
  'Standard_F2s'
  'Standard_F2s_v2'
  'Standard_F32s_v2'
  'Standard_F4'
  'Standard_F48s_v2'
  'Standard_F4s'
  'Standard_F4s_v2'
  'Standard_F64s_v2'
  'Standard_F72s_v2'
  'Standard_F8'
  'Standard_F8s'
  'Standard_F8s_v2'
  'Standard_FX12mds'
  'Standard_FX24mds'
  'Standard_FX36mds'
  'Standard_FX48mds'
  'Standard_FX4mds'
  'Standard_G1'
  'Standard_G2'
  'Standard_G3'
  'Standard_G4'
  'Standard_G5'
  'Standard_GS1'
  'Standard_GS2'
  'Standard_GS3'
  'Standard_GS4'
  'Standard_GS4-4'
  'Standard_GS4-8'
  'Standard_GS5'
  'Standard_GS5-16'
  'Standard_GS5-8'
  'Standard_H16'
  'Standard_H16_Promo'
  'Standard_H16m'
  'Standard_H16m_Promo'
  'Standard_H16mr'
  'Standard_H16mr_Promo'
  'Standard_H16r'
  'Standard_H16r_Promo'
  'Standard_H8'
  'Standard_H8_Promo'
  'Standard_H8m'
  'Standard_H8m_Promo'
  'Standard_HB120-16rs_v3'
  'Standard_HB120-32rs_v3'
  'Standard_HB120-64rs_v3'
  'Standard_HB120-96rs_v3'
  'Standard_HB120rs_v2'
  'Standard_HB120rs_v3'
  'Standard_HB60rs'
  'Standard_HC44rs'
  'Standard_L16s'
  'Standard_L16s_v2'
  'Standard_L32s'
  'Standard_L32s_v2'
  'Standard_L48s_v2'
  'Standard_L4s'
  'Standard_L64s_v2'
  'Standard_L80s_v2'
  'Standard_L8s'
  'Standard_L8s_v2'
  'Standard_M128'
  'Standard_M128-32ms'
  'Standard_M128-64ms'
  'Standard_M128dms_v2'
  'Standard_M128ds_v2'
  'Standard_M128m'
  'Standard_M128ms'
  'Standard_M128ms_v2'
  'Standard_M128s'
  'Standard_M128s_v2'
  'Standard_M16-4ms'
  'Standard_M16-8ms'
  'Standard_M16ms'
  'Standard_M192idms_v2'
  'Standard_M192ids_v2'
  'Standard_M192ims_v2'
  'Standard_M192is_v2'
  'Standard_M208ms_v2'
  'Standard_M208s_v2'
  'Standard_M32-16ms'
  'Standard_M32-8ms'
  'Standard_M32dms_v2'
  'Standard_M32ls'
  'Standard_M32ms'
  'Standard_M32ms_v2'
  'Standard_M32ts'
  'Standard_M416-208ms_v2'
  'Standard_M416-208s_v2'
  'Standard_M416ms_v2'
  'Standard_M416s_v2'
  'Standard_M64'
  'Standard_M64-16ms'
  'Standard_M64-32ms'
  'Standard_M64dms_v2'
  'Standard_M64ds_v2'
  'Standard_M64ls'
  'Standard_M64m'
  'Standard_M64ms'
  'Standard_M64ms_v2'
  'Standard_M64s'
  'Standard_M64s_v2'
  'Standard_M8-2ms'
  'Standard_M8-4ms'
  'Standard_M8ms'
  'Standard_NC12'
  'Standard_NC12_Promo'
  'Standard_NC12s_v2'
  'Standard_NC12s_v3'
  'Standard_NC16as_T4_v3'
  'Standard_NC24'
  'Standard_NC24_Promo'
  'Standard_NC24r'
  'Standard_NC24r_Promo'
  'Standard_NC24rs_v2'
  'Standard_NC24rs_v3'
  'Standard_NC24s_v2'
  'Standard_NC24s_v3'
  'Standard_NC4as_T4_v3'
  'Standard_NC6'
  'Standard_NC6_Promo'
  'Standard_NC64as_T4_v3'
  'Standard_NC6s_v2'
  'Standard_NC6s_v3'
  'Standard_NC8as_T4_v3'
  'Standard_ND12s'
  'Standard_ND24rs'
  'Standard_ND24s'
  'Standard_ND40rs_v2'
  'Standard_ND6s'
  'Standard_ND96asr_v4'
  'Standard_NV12'
  'Standard_NV12s_v2'
  'Standard_NV12s_v3'
  'Standard_NV16as_v4'
  'Standard_NV24'
  'Standard_NV24s_v2'
  'Standard_NV24s_v3'
  'Standard_NV32as_v4'
  'Standard_NV48s_v3'
  'Standard_NV4as_v4'
  'Standard_NV6'
  'Standard_NV6s_v2'
  'Standard_NV8as_v4'
])
param vmSku string = 'Standard_B4ms'

param instanceCount int

param diskSize int = 127

param adminUsername string

@secure()
param adminPassword string

param customScriptUrl string = 'https://raw.githubusercontent.com/iwate/saburaiis/main/scripts/custom.ps1'

param customScriptCommand string = 'powershell -ExecutionPolicy Unrestricted -File custom.ps1'

param saburaiisCoreName string

param workspaceId string

param workspaceKey string

param vnetRGName string

param vnetName string

param subnetName string

resource publicIp 'Microsoft.Network/publicIPAddresses@2021-02-01' = {
  name: '${name}-pip'
  location: resourceGroup().location
  sku: {
    name: 'Standard'
  }
  properties: {
    publicIPAllocationMethod: 'Static'
  }
}

resource lb 'Microsoft.Network/loadBalancers@2021-02-01' = {
  name: '${name}-lb'
  location: resourceGroup().location
  sku: {
    name: 'Standard'
  }
  properties: {
    frontendIPConfigurations: [
      {
        name: '${name}-frontend'
        properties: {
          publicIPAddress: {
            id: publicIp.id
          }
        }
      }
    ]
    backendAddressPools: [
      {
        name: '${name}-backend'
      }
    ]
    loadBalancingRules: [
      {
        name: '${name}-lb-http-rule'
        properties: {
          frontendIPConfiguration: {
            id: resourceId('Microsoft.Network/loadBalancers/frontendIPConfigurations', '${name}-lb', '${name}-frontend')
          }
          backendAddressPool: {
            id: resourceId('Microsoft.Network/loadBalancers/backendAddressPools', '${name}-lb', '${name}-backend')
          }
          protocol: 'Tcp'
          frontendPort: 80
          backendPort: 80
          enableFloatingIP: false
          idleTimeoutInMinutes: 5
          disableOutboundSnat: true
          probe:{
            id: resourceId('Microsoft.Network/loadBalancers/probes', '${name}-lb', '${name}-probe')
          }
        }
      }
      {
        name: '${name}-lb-https-rule'
        properties: {
          frontendIPConfiguration: {
            id: resourceId('Microsoft.Network/loadBalancers/frontendIPConfigurations', '${name}-lb', '${name}-frontend')
          }
          backendAddressPool: {
            id: resourceId('Microsoft.Network/loadBalancers/backendAddressPools', '${name}-lb', '${name}-backend')
          }
          protocol: 'Tcp'
          frontendPort: 443
          backendPort: 443
          enableFloatingIP: false
          idleTimeoutInMinutes: 5
          disableOutboundSnat: true
          probe:{
            id: resourceId('Microsoft.Network/loadBalancers/probes', '${name}-lb', '${name}-probe')
          }
        }
      }
    ]
    probes: [
      {
        name: '${name}-probe'
        properties: {
          protocol: 'Http'
          port: 80
          intervalInSeconds: 5
          numberOfProbes: 2
          requestPath: '/'
        }
      }
    ]
    inboundNatPools: [
    ]
    outboundRules: [
      {
        name: '${name}-lb-outbound-rule'
        properties: {
          allocatedOutboundPorts: 6392
          protocol: 'All'
          enableTcpReset: true
          idleTimeoutInMinutes: 4
          backendAddressPool: {
            id: resourceId('Microsoft.Network/loadBalancers/backendAddressPools', '${name}-lb', '${name}-backend')
          }
          frontendIPConfigurations: [
            {
              id: resourceId('Microsoft.Network/loadBalancers/frontendIPConfigurations', '${name}-lb', '${name}-frontend')
            }
          ]
        }
      }
    ]
  }
  dependsOn: [
    publicIp
  ]
}

resource nsg 'Microsoft.Network/networkSecurityGroups@2020-11-01' = {
  name: '${name}-nsg'
  location: resourceGroup().location
  properties: {
    securityRules: [
      {
        name: 'Http'
        properties: {
          protocol: 'Tcp'
          sourcePortRange: '*'
          destinationPortRange: '80'
          sourceAddressPrefix: '*'
          destinationAddressPrefix: '*'
          access: 'Allow'
          priority: 100
          direction: 'Inbound'
        }
      }
      {
        name: 'Https'
        properties: {
          protocol: 'Tcp'
          sourcePortRange: '*'
          destinationPortRange: '443'
          sourceAddressPrefix: '*'
          destinationAddressPrefix: '*'
          access: 'Allow'
          priority: 110
          direction: 'Inbound'
        }
      }
    ]
  }
}

resource vmss 'Microsoft.Compute/virtualMachineScaleSets@2021-04-01' = {
  name: name
  location: resourceGroup().location
  sku: {
    name: vmSku
    capacity: instanceCount
  }
  zones: pickZones('Microsoft.Compute', 'virtualMachines', resourceGroup().location, 2)
  identity: {
    type:'SystemAssigned'
  }
  properties: {
    overprovision: true
    upgradePolicy: {
      mode: 'Manual'
    }
    scaleInPolicy: {
      rules: [
        'OldestVM'
      ]
    }
    virtualMachineProfile: {
      storageProfile: {
        osDisk: {
          createOption: 'FromImage'
          caching: 'ReadWrite'
          managedDisk: {
            storageAccountType: 'StandardSSD_LRS'
          }
          diskSizeGB: diskSize
        }
        imageReference: {
          publisher: 'MicrosoftWindowsServer'
          offer: 'WindowsServer'
          sku: '2019-datacenter-gensecond'
          version: 'latest'
        }
      }
      osProfile: {
        computerNamePrefix: vmPrefix
        adminUsername: adminUsername
        adminPassword: adminPassword
        windowsConfiguration: {
          provisionVMAgent: true
          enableAutomaticUpdates: true
        }
      }
      networkProfile: {
        networkInterfaceConfigurations: [
          {
            name: '${name}-nic'
            properties: {
              primary: true
              ipConfigurations: [
                {
                  name: 'ipconfig'
                  properties: {
                    subnet: {
                      id: resourceId(vnetRGName,'Microsoft.Network/virtualNetworks/subnets', vnetName, subnetName)
                    }
                    loadBalancerBackendAddressPools: [
                      {
                        id: resourceId('Microsoft.Network/loadBalancers/backendAddressPools', '${name}-lb', '${name}-backend')
                      }
                    ]
                    loadBalancerInboundNatPools: [
                    ]
                  }
                }
              ]
              networkSecurityGroup: {
                id: nsg.id
              }
            }
          }
        ]
      }
      diagnosticsProfile: {
        bootDiagnostics: {
          enabled: true
        }
      }
      extensionProfile: {
        extensions: [
          {
            name: 'CustomScriptExtension'
            properties: {
              publisher: 'Microsoft.Compute'
              type: 'CustomScriptExtension'
              typeHandlerVersion: '1.10'
              autoUpgradeMinorVersion: true
              protectedSettings: {
                fileUris: [
                  'https://raw.githubusercontent.com/iwate/saburaiis/main/scripts/provisioning.ps1'
                  customScriptUrl
                ]
                commandToExecute: 'powershell -ExecutionPolicy Unrestricted -File provisioning.ps1 -ScaleSetName ${name} -SubscriptionId ${subscription().subscriptionId} -ResourceGroupName ${saburaiisCoreName} && ${customScriptCommand}'
              }
            }
          }
          {
            name: 'HealthExtension'
            properties: {
              autoUpgradeMinorVersion: true
              publisher: 'Microsoft.ManagedServices'
              type: 'ApplicationHealthWindows'
              typeHandlerVersion: '1.0'
              settings: {
                protocol: 'http'
                port: 80
                requestPath: '/'
              }
            }
          }
          {
            name: 'MonitoringExtension'
            properties: {
              autoUpgradeMinorVersion: true
              publisher: 'Microsoft.EnterpriseCloud.Monitoring'
              type: 'MicrosoftMonitoringAgent'
              typeHandlerVersion: '1.0'
              settings: {
                workspaceId: workspaceId
              }
              protectedSettings: {
                workspaceKey: workspaceKey
              }
            }
          }
        ]
      }
    }
  }
  dependsOn: [
    lb
  ]
}

output principalId string = vmss.identity.principalId
