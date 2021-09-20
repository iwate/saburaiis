# SaburaIIS

Pulling type deploy service for IIS.

## Motivation

Azure VMSS is so attractive. However, Kubernetes and Service Fabric are too rich for the people who use simpley web server management that copy app directory to IIS Server and changing binded folder.  
SaburaIIS is a solution for who need more simple way.

## How to Install

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

### Step 3 - Deploy SaburaIIS Partition(VMSS)

Create a new partition resource.

[![Deploy To Azure](https://raw.githubusercontent.com/Azure/azure-quickstart-templates/master/1-CONTRIBUTION-GUIDE/images/deploytoazure.svg?sanitize=true)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2Fiwate%2Fsaburaiis%2Fmaster%2Fdeployment%2Fpartition.json)


### Step 4 - Assign IAM Roles
[![Deploy To Azure](https://raw.githubusercontent.com/Azure/azure-quickstart-templates/master/1-CONTRIBUTION-GUIDE/images/deploytoazure.svg?sanitize=true)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2Fiwate%2Fsaburaiis%2Fmaster%2Fdeployment%2Fiam.json)