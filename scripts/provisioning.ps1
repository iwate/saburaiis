[CmdletBinding(DefaultParameterSetName="ManagedIdentity")]
param (
    [Parameter(ParameterSetName = "ManagedIdentity",  Mandatory=$true, HelpMessage = "Enter the name of the ScaleSet to which it belong.")]
    [Parameter(ParameterSetName = "ServicePrincipal", Mandatory=$true, HelpMessage = "Enter the name of the ScaleSet to which it belong.")]
    [string] 
    $ScaleSetName,

    [Parameter(ParameterSetName = "ManagedIdentity",  Mandatory=$true, HelpMessage = "Enter azure subscription id of CosmosDB and KeyVault.")]
    [Parameter(ParameterSetName = "ServicePrincipal",  Mandatory=$true, HelpMessage = "Enter azure subscription id of CosmosDB and KeyVault.")]
    [string] 
    $SubscriptionId,

    [Parameter(ParameterSetName = "ManagedIdentity",  Mandatory=$true, HelpMessage = "Enter resource group name of CosmosDB and KeyVault.")]
    [Parameter(ParameterSetName = "ServicePrincipal",  Mandatory=$true, HelpMessage = "Enter resource group name of CosmosDB and KeyVault.")]
    [string] 
    $ResourceGroupName,

    [Parameter(ParameterSetName = "ManagedIdentity",  Mandatory=$true, HelpMessage = "Enter CosmosDB instance name.")]
    [Parameter(ParameterSetName = "ServicePrincipal",  Mandatory=$true, HelpMessage = "Enter CosmosDB instance name.")]
    [string] 
    $CosmosDBName,

    [Parameter(ParameterSetName = "ManagedIdentity",  Mandatory=$true, HelpMessage = "Enter KeyVault instance name.")]
    [Parameter(ParameterSetName = "ServicePrincipal",  Mandatory=$true, HelpMessage = "Enter KeyVault instance name.")]
    [string] 
    $KeyVaultName,

    [Parameter(ParameterSetName = "ServicePrincipal", Mandatory=$false, HelpMessage = "Enter tenant id of service principal.")]
    [string] 
    $AADTenantId,

    [Parameter(ParameterSetName = "ServicePrincipal", Mandatory=$false, HelpMessage = "Enter client id of service principal.")]
    [string] 
    $AADClientId,

    [Parameter(ParameterSetName = "ServicePrincipal", Mandatory=$false, HelpMessage = "Enter client secret of service principal.")]
    [string] 
    $AADClientSecret,

    [Parameter(ParameterSetName = "ManagedIdentity",  Mandatory=$false, HelpMessage = "Enter SaburaIIS release version.")]
    [Parameter(ParameterSetName = "ServicePrincipal",  Mandatory=$false, HelpMessage = "Enter SaburaIIS release version.")]
    [string] 
    $Version = "latest"
)

$ErrorActionPreference = 'Stop'

# Install windows features

Install-WindowsFeature -Name Web-Server -IncludeManagementTools

"SaburaIIS" | Out-File -FilePath c:\inetpub\wwwroot\saburaiis.txt

# Resolve download link
$DownloadLink = "https://github.com/iwate/saburaiis/releases/latest/download/SaburaIIS.Agent.zip"

if ($Version -ne "latest") {
    $DownloadLink = "https://github.com/iwate/saburaiis/releases/download/$Version/SaburaIIS.Agent.zip"
}

# Download agent binary
Invoke-RestMethod -Method Get -Uri $DownloadLink -OutFile $env:temp\saburaiis.zip
Expand-Archive -Path $env:temp\saburaiis.zip -DestinationPath $env:SystemDrive\saburaiis

# Create service
$LogLevel = "--Logging:EventLog:LogLevel:Microsoft=Information --Logging:EventLog:LogLevel:SaburaIIS=Information"

$ResourceArguments = "--SaburaIIS:SubscriptionId=$SubscriptionId --SaburaIIS:ResourceGroupName=$ResourceGroupName --SaburaIIS:CosmosDbName=$CosmosDbName --SaburaIIS:KeyVaultName=$KeyVaultName"
$ServicePrincipalArguments = "--SaburaIIS:AADTenantId=$AADTenantId --SaburaIIS:AADClientId=$AADClientId --SaburaIIS:AADClientSecret=$AADClientSecret"
$Arguments = "--SaburaIIS:ScaleSetName=$ScaleSetName $ResourceArguments $ServicePrincipalArguments"

sc.exe create "SaburaIIS" binpath="$env:SystemDrive\saburaiis\SaburaIIS.Agent.exe $Arguments $LogLevel" start=auto

# Start service
sc.exe start "SaburaIIS"