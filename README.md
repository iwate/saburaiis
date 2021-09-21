# SaburaIIS

Pulling type deploy service for IIS.

## Motivation

Azure VMSS is so attractive. However, Kubernetes and Service Fabric are too rich for the people who use simpley web server management that copy app directory to IIS Server and changing binded folder.  
SaburaIIS is a solution for who need more simple way.

## Concept

An orchestrator usualy consists of a few master nodes and many workder nodes.   
Master nodes communicate each other to manage state all system.   
Multi Paxos and Raft are famous algorithm for make masters.   
However, as you know, managed Kubernetes is very popular. So, no one wants to manage the masters.

This project's concept "Are azure managed services able to make it simple?"

- Using CosmosDB insted of master nodes
- Using VMSS and Helth extension to ensure resiliency

![image](https://user-images.githubusercontent.com/1011232/134011966-d70b2737-81f0-412d-afd4-eda56c997e47.png)

Cosmos Db has a data what applicationHost.config should be. And saburaiis agents update IIS on each VM. This relationship like a relationship between VirtualDOM and DOM.

![image](https://user-images.githubusercontent.com/1011232/134018841-a64dc31f-41ce-4a3c-ae10-6c5ae66068d0.png)

## Architecture

![image](https://user-images.githubusercontent.com/1011232/134033034-d72cd163-fcda-41f8-acdd-1af2a5f5f5bf.png)

- Cosmos DB: Store data what IIS should be.
- Storage: Store applciation package(zip).
- Key Vault: Manage SSL certificate.
- Log Analytics: Monitoring saburaiis agent logs.
- Partition(VMSSs): Worker group. Partition has a few VMSS.
- SaburaIIS Agent: Update IIS when Cosmos DB change trigger fired.
- SaburaIIS CLI: Manage data stored in Cosmos DB and Storage.
- SaburaIIS AdminWeb: Manage data stored in Cosmos DB and Storage.

SaburaIIS can configure multi partitions and multi regions.

![image](https://user-images.githubusercontent.com/1011232/134007767-a40fb0ee-db05-4745-adfe-60a3ae07571f.png)

## Tutorial

### Step 1 - Create Service Principal

Create a new service principal and get its object ID(oooooooo-oooo-oooo-oooo-oooooooooooo).

```
$ az ad sp create-for-rbac --skip-assignment -n "SaburaIIS"
{
  "appId": "nnnnnnnn-nnnn-nnnn-nnnn-nnnnnnnnnnnn",
  "displayName": "SaburaIIS",
  "name": "nnnnnnnn-nnnn-nnnn-nnnn-nnnnnnnnnnnn",
  "password": "ppppppppppppppppppppppppppppppppp",
  "tenant": "tttttttt-tttt-tttt-tttt-tttttttttttt"
}
$ az ad sp show --query objectId --id nnnnnnnn-nnnn-nnnn-nnnn-nnnnnnnnnnnn
"oooooooo-oooo-oooo-oooo-oooooooooooo"
```

### Step 2 - Deploy Resources

Create core resources (CosmoDB, KeyVault, Storage), vnet and a partition (VMSS, PIP, LB, NSG)

[![Deploy To Azure](https://raw.githubusercontent.com/Azure/azure-quickstart-templates/master/1-CONTRIBUTION-GUIDE/images/deploytoazure.svg?sanitize=true)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2Fiwate%2Fsaburaiis%2Fmaster%2Fdeployment%2Fsaburaiis.json)

### Step 3 - Setup WebAdmin

Download SaburaIIS.WebAdmin.zip [latest release](https://github.com/iwate/saburaiis/releases/latest)

Extract zip and configure `appsettings.json`

```
{
  "Logging": {
    "LogLevel": {
    "Default": "Information",
    "Microsoft": "Warning",
    "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*",
  "SaburaIIS": {
    "SubscriptionId": "<Your subscription id>",
    "ResourceGroupName": "<Core resource group name(which has cosmosdb, keyvault and storage)>"
  }
}
```

AdminWeb use managed identity to access resources. You need setup on visual studio (https://docs.microsoft.com/en-us/azure/app-service/app-service-web-tutorial-connect-msi?tabs=windowsclient%2Cdotnet#set-up-visual-studio)

Also, You use service principal insted of managed identity if you want to.

```
{
  ...
  "SaburaIIS": {
    "SubscriptionId": "<Your subscription id>",
    "ResourceGroupName": "<Core resource group name(which has cosmosdb, keyvault and storage)>",
    "AADTenantId": "<Your tenant id>",
    "AADClientId": "<Service Principal appId(name)>",
    "AADClientSecret": "<Service Principal password>"
  }
}
```

Execute SaburaIIS.WebAdmin.exe and access https://localhost:5001/ on your browser.

### Step 4 Create Partition settings on WebAdmin

Click `New Partition` button and input new partition name, finaly click the `Create` button.
![image](https://user-images.githubusercontent.com/1011232/134100077-031f53d4-d614-4ae7-9589-0e7c562364a4.png)

### Step 5 Participate VMSS in the Partition

1. Click `Manage Scale Set` button
1. Click `Add Scale Set` button
1. Input VMSS name which created in step 2
1. Stage changes

![image](https://user-images.githubusercontent.com/1011232/134106740-cc98b4b3-8a4c-4d3f-b2bc-f7d1184065f6.png)

Check current changes and apply it.

![image](https://user-images.githubusercontent.com/1011232/134106859-1818cac7-372f-413d-85df-5e71ba5524f0.png)

Navigate to `Instances` and wait active instances

![image](https://user-images.githubusercontent.com/1011232/134114191-9059a3f1-ff5a-41fb-b918-36060b804790.png)

### Step 6 - Setup CLI

Install CLI tool via dotnet tool command and set env values.

```
PS> dotnet tool install -g SaburaIIS.CLI
PS> $env:AZURE_SUBSCRIPTION_ID="<Your subscription id>"
PS> $env:SABURAIIS_RG_NAME="<Core resource group name(which has cosmosdb, keyvault and storage)>"
PS> $env:AZURE_TENANT_ID="<Your tenant id>"
PS> $env:AZURE_CLIENT_ID="<Service Principal appId(name)>"
PS> $env:AZURE_CLIENT_SECRET="<Service Principal password>"
```
### Step 7 - Upload Application Package

Create simple applciation package(zip) and execute `saburaiis release` command.
```
PS> echo "SaburaIIS" > index.html
PS> Compress-Archive -Path .\index.html -DestinationPath index.zip
PS> saburaiis release EmptySite v0.0.1 --zip .\index.zip
```

### Step 8 - Deploy Application

Modify application physical path and update partition settings. saburaiis agent parse last two directories of  physical path are package name and version. (`%SystemDrive%\inetpub\site\{package name}\{version}`)

```
PS> saburaiis export partition01 --output ./partition01.json
PS> saburaiis modify-path "Default Web Site" "/" EmptySite v0.0.1 .\partition01.json
PS> cat .\partition01.json
...
              "PhysicalPath": "%SystemDrive%\\inetpub\\sites\\EmptySite\\v0.0.1",
...
PS> saburaiis import partition01 ./partition01.json
```

After update partition settings, instances state will change on AdminWeb.

![image](https://user-images.githubusercontent.com/1011232/134116148-7cf1c8e1-5043-418b-9f98-7c9a7681de04.png)

You can get the simple application at Public IP of VMSS.

![image](https://user-images.githubusercontent.com/1011232/134116287-5ba1a12e-8b99-4b58-acf4-bacce1270de1.png)


## How to Add Partition

### Step 1 - Create New Subnet
Create new subnet in portal.  
And copy vnet resource group name, vnet name and subnet.

### Step 2 - Deploy Partition
Create a new partition resource.

[![Deploy To Azure](https://raw.githubusercontent.com/Azure/azure-quickstart-templates/master/1-CONTRIBUTION-GUIDE/images/deploytoazure.svg?sanitize=true)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2Fiwate%2Fsaburaiis%2Fmaster%2Fdeployment%2Fpartition.json)

### Step 3 - Assign IAM Roles

Assign IAM roles to vmss created in step 2.

Required parameters

- Core resource group name
- Principal ID of created new vmss
- CosmosDB name within core resource group
- Key Vault name within core resource group
- Pakcages container name within storage account within core resouce group.  
The format is `${storage account name}/defaults/${blob container name = 'packages'}` like as `saburaiis/defaults/packages`

packages container name is the format `${storage name}/defaults/packages`.


[![Deploy To Azure](https://raw.githubusercontent.com/Azure/azure-quickstart-templates/master/1-CONTRIBUTION-GUIDE/images/deploytoazure.svg?sanitize=true)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2Fiwate%2Fsaburaiis%2Fmaster%2Fdeployment%2Fiam.json)

## How to Add Partition in Another Region (Adding Region)

### Step 1 Create New VNET

Create a new vnet in the new region.

### Step 2
[How to Add Partition](#how-to-add-partition)
