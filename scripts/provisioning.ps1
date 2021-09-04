param (
    [Parameter(Mandatory=$true, HelpMessage = "Enter the name of the ScaleSet to which it belong.")]
    [string] 
    $ScaleSetName,

    [Parameter(Mandatory=$true, HelpMessage = "Enter azure subscription id of CosmosDB and KeyVault.")]
    [string] 
    $SubscriptionId,

    [Parameter(Mandatory=$true, HelpMessage = "Enter resource group name of CosmosDB and KeyVault.")]
    [string] 
    $ResourceGroupName,

    [Parameter(Mandatory=$true, HelpMessage = "Enter CosmosDB instance name.")]
    [string] 
    $CosmosDBName,

    [Parameter(Mandatory=$true, HelpMessage = "Enter KeyVault instance name.")]
    [string] 
    $KeyVaultName,

    [Parameter(Mandatory=$true, HelpMessage = "Enter tenant id of service principal.")]
    [string] 
    $AADTenantId,

    [Parameter(Mandatory=$true, HelpMessage = "Enter client id of service principal.")]
    [string] 
    $AADClientId,

    [Parameter(Mandatory=$true, HelpMessage = "Enter client secret of service principal.")]
    [string] 
    $AADClientSecret
)

$ErrorActionPreference = 'Stop'

# Download agent binary
Invoke-RestMethod -Method Get -Uri "https://github.com/iwate/saburaiis/releases/latest/download/SaburaIIS.Agent.zip" -OutFile $env:temp\saburaiis.zip
Expand-Archive -Path $env:temp\saburaiis.zip -DestinationPath $env:SystemDrive\saburaiis

# Create service
$LogLevel = "--Logging:EventLog:LogLevel:Microsoft=Information --Logging:EventLog:LogLevel:SaburaIIS=Information"

$ResourceArguments = "--SaburaIIS:SubscriptionId=$SubscriptionId --SaburaIIS:ResourceGroupName=$ResourceGroupName --SaburaIIS:CosmosDbName=$CosmosDbName --SaburaIIS:KeyVaultName=$KeyVaultName"
$ServicePrincipalArguments = "--SaburaIIS:AADTenantId=$AADTenantId --SaburaIIS:AADClientId=$AADClientId --SaburaIIS:AADClientSecret=$AADClientSecret"
$Arguments = "--SaburaIIS:ScaleSetName=$ScaleSetName $ResourceArguments $ServicePrincipalArguments"

sc.exe create "SaburaIIS" binpath="$env:SystemDrive\saburaiis\SaburaIIS.Agent.exe $Arguments $LogLevel"

# Start service
sc.exe start "SaburaIIS"