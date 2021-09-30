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

Cosmos DB has a data what applicationHost.config should be. And saburaiis agents update IIS on each VM. This relationship like a relationship between VirtualDOM and DOM.

![image](https://user-images.githubusercontent.com/1011232/134018841-a64dc31f-41ce-4a3c-ae10-6c5ae66068d0.png)

## Architecture

![saburaiis-Page-2](https://user-images.githubusercontent.com/1011232/135226302-ca580d75-5759-4770-961a-a557795f0a22.jpg)


- Cosmos DB: Store data what IIS should be.
- Storage: Store applciation package(zip).
- Key Vault: Manage SSL certificate.
- App Configuration: Manage environment variables for Application Pool.
- Log Analytics: Monitoring saburaiis agent logs.
- Partition(VMSSs): Worker group. Partition has a few VMSS.
- SaburaIIS Agent: Update IIS when Cosmos DB change trigger fired.
- SaburaIIS CLI: Manage data stored in Cosmos DB and Storage.
- SaburaIIS AdminWeb: Manage data stored in Cosmos DB and Storage.

SaburaIIS can configure multi partitions and multi regions.

![saburaiis-Page-1](https://user-images.githubusercontent.com/1011232/135226525-123e7d74-4f29-43d6-9d74-ba8caa940c99.jpg)


## Get Started

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
![image](https://user-images.githubusercontent.com/1011232/134123773-4f55b905-8672-407a-8056-fc64e92df50d.png)

### Step 5 Participate VMSS in the Partition

1. Click `Manage Scale Set` button
1. Click `Add Scale Set` button
1. Input VMSS name which created in step 2
1. Stage changes

![image](https://user-images.githubusercontent.com/1011232/134123920-3822c5cb-6f33-4a05-bb87-ac4ab6472492.png)

Check current changes and apply it.

![image](https://user-images.githubusercontent.com/1011232/134124082-b9e8ef4e-9a35-406f-9d20-708e5db76825.png)

Navigate to `Instances` and wait active instances

![image](https://user-images.githubusercontent.com/1011232/134124285-4e7226da-4f0f-4b02-a4d7-cb325ab4ca71.png)

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

## How to Manage SSL Certificate

### Step 1 - Add Certtificate into KeyVault

Generate or import certificate to key vault which is created at [Get Started - Step 2](#step-2---deploy-resources).
If you don't have roles for edit, add IAM roles (ex. Key Vault Administrator) your account.

![image](https://user-images.githubusercontent.com/1011232/135227613-0f9f4287-4fdb-4529-b405-bec74b333e27.png)

### Step 2 - Modify Site Binding

Add new binding to partition on AdminWeb. And select certificate store name and certificate hash.

![image](https://user-images.githubusercontent.com/1011232/135229178-ee014289-71aa-48c5-8858-f8fa65fd69d9.png)

After stage, apply changes and reload your site by https.

## How to Manage App Configuration

### Step 1 - Add Key Value into App Configuration

![image](https://user-images.githubusercontent.com/1011232/135229691-cae5dbe1-f5f2-499c-b4c7-878113a0c8da.png)

You can use [Key Vault Acmebot](https://github.com/shibayan/keyvault-acmebot) if you want to use Let's Encrypt or other acme issuer.

### Step 2 - Recycle Application Pool

Update application pool recycle request on AdminWeb.

![image](https://user-images.githubusercontent.com/1011232/135229899-fc007399-c3d1-4cbe-af2b-2991f1041e0f.png)

After stage, apply changes and reload your site.

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
