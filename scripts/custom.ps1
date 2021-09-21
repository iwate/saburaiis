param(
    $DotNetCoreDownloadLink = "https://download.visualstudio.microsoft.com/download/pr/c154f72b-dcaf-4852-8fbe-20f0f3c779a7/91c7e664a755fb3142740f14c5c96ea7/dotnet-hosting-3.1.19-win.exe",
    $FunctionsDownloadLink = "https://github.com/Azure/azure-functions-host/releases/download/v3.2.0/Functions.3.2.0.zip",
    $FunctionsRuntimePath = "c:\inetpub\site\.functions"
)
$ErrorActionPreference = "Stop"

# Install dotnet core hosting bundle
$DotNetCoreInstaller = "$env:temp\dotnetcore-installer.exe";
Invoke-RestMethod -Method Get -Uri $DotNetCoreDownloadLink -OutFile $DotNetCoreInstaller
Start-Process $DotNetCoreInstaller -ArgumentList "/install", "/quiet", "/norestart" -NoNewWindow -Wait

# Deploy function host runtime
$FunctionsPackage = "$env:temp\functions.zip";
Invoke-RestMethod -Method Get -Uri $FunctionsDownloadLink -OutFile $FunctionsPackage
Expand-Archive -Path $FunctionsPackage -DestinationPath $FunctionsRuntimePath

# Restart web server service
net stop was /y
net start w3svc
