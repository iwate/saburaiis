﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using SaburaIIS;
using SaburaIIS.CLI;
using SaburaIIS.Json;
using SaburaIIS.Models;
using SaburaIIS.Validations;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

var rootCommand = new RootCommand("SaburaIIS command-line tool");

rootCommand.Name = "saburaiis";

rootCommand.AddGlobalOption(new Option<string?>("--subscription") 
{
    Description = "Subscription id (fallback to AZURE_SUBSCRIPTION_ID environment variable)",
    IsRequired = false
});
rootCommand.AddGlobalOption(new Option<string?>("--resource-group")
{
    Description = "Resource Group name (fallback to SABURAIIS_RG_NAME environment variable)",
    IsRequired = false
});
rootCommand.AddGlobalOption(new Option<string?>("--cosmosdb-name")
{ 
    Description= "CosmosDB name for state store (fallback to SABURAIIS_DB_NAME or SABURAIIS_RG_NAME environment variable)",
    IsRequired = false
});
rootCommand.AddGlobalOption(new Option<string?>("--cosmosdb-endpoint") 
{
    Description = "CosmosDB endpoint (fallback to SABURAIIS_DB_ENDPOINT environment variable or 'https://{--cosmosdb-name}.documents.azure.com/')",
    IsRequired = false 
});
rootCommand.AddGlobalOption(new Option<string?>("--storage-name")
{
    Description = "Storage Account name for package storage (fallback to SABURAIIS_STORAGE_NAME or SABURAIIS_RG_NAME environment variable)",
    IsRequired = false
});
rootCommand.AddGlobalOption(new Option<string?>("--storage-container-name", getDefaultValue: () => "packages")
{
    Description = "Storage Blob Container name (fallback to SABURAIIS_STORAGE_CONTAINER_NAME environment variable)",
    IsRequired = false,
});
rootCommand.AddGlobalOption(new Option<string?>("--storage-container-endpoint")
{
    Description = "Storage Blob Container endpoint for package storage (fallback to SABURAIIS_STORAGE_CONTAINER_ENDPOINT environment variable or 'https://{--storage-name}.blob.core.windows.net/{--storage-container-name}/')",
    IsRequired = false
});
rootCommand.AddGlobalOption(new Option<string?>("--tenant-id") 
{
    Description = "Service principal tenant id (fallback to AZURE_TENANT_ID environment variable)",
    IsRequired = false
});
rootCommand.AddGlobalOption(new Option<string?>("--client-id") 
{ 
    Description = "Service principal client id (fallback to AZURE_CLIENT_ID environment variable)",
    IsRequired = false
});
rootCommand.AddGlobalOption(new Option<string?>("--client-secret") 
{
    Description = "Service principal client secret (fallback to AZURE_CLIENT_SECRET environment variable)",
    IsRequired = false 
});
rootCommand.AddGlobalOption(new Option<string?>("--file-store-path")
{
    Description = "Directory path for state store",
    IsRequired = false
});
rootCommand.AddGlobalOption(new Option<string?>("--file-storage-path")
{
    Description = "Directory path for Package storage",
    IsRequired = false
});
rootCommand.AddGlobalOption(new Option<bool?>("--use-machine-vault")
{
    Description = "Use machine certificate store for certificate vault",
    IsRequired = false
});

var jsonOptions = new JsonSerializerOptions
{
    WriteIndented = true,
};
jsonOptions.Converters.Add(new JsonStringEnumConverter());
jsonOptions.Converters.Add(new TimeSpanConverter());
jsonOptions.Converters.Add(new BinaryConverter());

var env = Environment.GetEnvironmentVariables();
string? envValue(string name) => env.Contains(name) ? (string?)env[name] : null;
string? fallback(params string?[] values) => values.FirstOrDefault(value => !string.IsNullOrEmpty(value));

var addPartitionCommand = new Command("add-partition", "Createing new partition")
{
    new Argument<string>("partition") { 
        Description = "A name of partition",
    }
};

addPartitionCommand.Handler = CommandHandler.Create(async (
    string partition,
    string? subscription,
    string? resourceGroup,
    string? cosmosdbName,
    string? cosmosdbEndpoint,
    string? tenantId,
    string? clientId,
    string? clientSecret,
    string? fileStorePath,
    string? fileStoragePath,
    bool? useMachineVault) =>
{
    var config = new Config
    {
        SubscriptionId    = fallback(subscription,     envValue("AZURE_SUBSCRIPTION_ID")) ?? throw new ArgumentNullException(nameof(subscription)),
        ResourceGroupName = fallback(resourceGroup,    envValue("SABURAIIS_RG_NAME"))     ?? throw new ArgumentNullException(nameof(resourceGroup)),
        CosmosDbName      = fallback(cosmosdbName,     envValue("SABURAIIS_DB_NAME")),
        CosmosDbEndpoint  = fallback(cosmosdbEndpoint, envValue("SABURAIIS_DB_ENDPOINT")),
        AADTenantId       = fallback(tenantId,         envValue("AZURE_TENANT_ID")),
        AADClientId       = fallback(clientId,         envValue("AZURE_CLIENT_ID")),
        AADClientSecret   = fallback(clientSecret,     envValue("AZURE_CLIENT_SECRET")),
        FileStoreDirectoryPath     = fallback(fileStorePath,     envValue("SABURAIIS_FILE_STORE_DIR_PATH")),
        FileStorageDirectoryPath   = fallback(fileStoragePath,   envValue("SABURAIIS_FILE_STORAGE_DIR_PATH")),
        UseMachineVault = bool.Parse(fallback(useMachineVault?.ToString(), envValue("SABURAIIS_USE_MACHINE_VAULT")) ?? "false"),
    };

    var store = Factory.CreateStore(config);

    await store.InitAsync();

    var (current, _) = await store.GetPartitionAsync(partition);

    if (current != null)
    {
        Console.WriteLine($"{partition} already exists");
        return;
    }

    await store.SavePartitionAsync(Defaults.CreatePartition(partition, config), "*");
});

rootCommand.AddCommand(addPartitionCommand);

var exportCommand = new Command("export", "Exporting partition settings to file.")
{
    new Argument<string>("partition") { 
        Description = "A name of partition",
        Arity = ArgumentArity.ExactlyOne
    },
    new Option<FileInfo?>("--output") { 
        Description =  "An output file path for export",
        IsRequired = false
    },
};

exportCommand.Handler = CommandHandler.Create(async (
    string partition,
    FileInfo output,
    string? subscription,
    string? resourceGroup,
    string? cosmosdbName,
    string? cosmosdbEndpoint,
    string? tenantId,
    string? clientId,
    string? clientSecret,
    string? fileStorePath,
    string? fileStoragePath,
    bool? useMachineVault) =>
{
    var config = new Config
    {
        SubscriptionId    = fallback(subscription,     envValue("AZURE_SUBSCRIPTION_ID")) ?? throw new ArgumentNullException(nameof(subscription)),
        ResourceGroupName = fallback(resourceGroup,    envValue("SABURAIIS_RG_NAME"))     ?? throw new ArgumentNullException(nameof(resourceGroup)),
        CosmosDbName      = fallback(cosmosdbName,     envValue("SABURAIIS_DB_NAME")),
        CosmosDbEndpoint  = fallback(cosmosdbEndpoint, envValue("SABURAIIS_DB_ENDPOINT")),
        AADTenantId       = fallback(tenantId,         envValue("AZURE_TENANT_ID")),
        AADClientId       = fallback(clientId,         envValue("AZURE_CLIENT_ID")),
        AADClientSecret   = fallback(clientSecret,     envValue("AZURE_CLIENT_SECRET")),
        FileStoreDirectoryPath     = fallback(fileStorePath,     envValue("SABURAIIS_FILE_STORE_DIR_PATH")),
        FileStorageDirectoryPath   = fallback(fileStoragePath,   envValue("SABURAIIS_FILE_STORAGE_DIR_PATH")),
        UseMachineVault = bool.Parse(fallback(useMachineVault?.ToString(), envValue("SABURAIIS_USE_MACHINE_VAULT")) ?? "false"),
    };

    var store = Factory.CreateStore(config);

    await store.InitAsync();

    var (data, etag) = await store.GetPartitionAsync(partition);

    if (data == null)
    {
        Console.WriteLine($"{partition} is not found.");
        return;
    }

    var json = JsonSerializer.Serialize(new PartitionWithETag 
    {
        ETag = etag,
        Name = data.Name,
        ApplicationPools = data.ApplicationPools,
        Sites = data.Sites,
        ScaleSets = data.ScaleSets
    }, jsonOptions);

    if (output == null)
    {
        Console.WriteLine(json);
    }
    else
    {
        await File.WriteAllTextAsync(output.FullName, json);
    }
});

rootCommand.AddCommand(exportCommand);

var importCommand = new Command("import", "Importing partition settings from file.")
{
    new Argument<string>("partition") { 
        Description = "A name of partition",
    },
    new Argument<FileInfo?>("input") { 
        Description =  "An input file path for import",
    },
};

importCommand.Handler = CommandHandler.Create(async (
    string partition,
    FileInfo input,
    string? subscription,
    string? resourceGroup,
    string? cosmosdbName,
    string? cosmosdbEndpoint,
    string? tenantId,
    string? clientId,
    string? clientSecret,
    string? fileStorePath,
    string? fileStoragePath,
    bool? useMachineVault) =>
{
    if (!input.Exists)
        throw new ArgumentException($"{nameof(input.FullName)} is not exists");

    var config = new Config
    {
        SubscriptionId    = fallback(subscription,     envValue("AZURE_SUBSCRIPTION_ID")) ?? throw new ArgumentNullException(nameof(subscription)),
        ResourceGroupName = fallback(resourceGroup,    envValue("SABURAIIS_RG_NAME"))     ?? throw new ArgumentNullException(nameof(resourceGroup)),
        CosmosDbName      = fallback(cosmosdbName,     envValue("SABURAIIS_DB_NAME")),
        CosmosDbEndpoint  = fallback(cosmosdbEndpoint, envValue("SABURAIIS_DB_ENDPOINT")),
        AADTenantId       = fallback(tenantId,         envValue("AZURE_TENANT_ID")),
        AADClientId       = fallback(clientId,         envValue("AZURE_CLIENT_ID")),
        AADClientSecret   = fallback(clientSecret,     envValue("AZURE_CLIENT_SECRET")),
        FileStoreDirectoryPath     = fallback(fileStorePath,     envValue("SABURAIIS_FILE_STORE_DIR_PATH")),
        FileStorageDirectoryPath   = fallback(fileStoragePath,   envValue("SABURAIIS_FILE_STORAGE_DIR_PATH")),
        UseMachineVault = bool.Parse(fallback(useMachineVault?.ToString(), envValue("SABURAIIS_USE_MACHINE_VAULT")) ?? "false"),
    };

    var store = Factory.CreateStore(config);

    await store.InitAsync();

    var (current, _) = await store.GetPartitionAsync(partition);

    if (current == null)
    {
        Console.WriteLine($"{partition} is not found.");
        return;
    }

    var json = await File.ReadAllTextAsync(input.FullName);
    var @new = JsonSerializer.Deserialize<PartitionWithETag>(json, jsonOptions);

    if (@new == null)
        throw new ArgumentException($"{input.Name} is invalid partition json file");

    if (@new.Name != partition)
        throw new ArgumentException($"{input.Name} is not for {partition}");

    await store.SavePartitionAsync(@new, @new.ETag);
});

rootCommand.AddCommand(importCommand);

var releaseCommand = new Command("release", "Releasing new package version.")
{
    new Argument<string>("package") { 
        Description = "A name of package",
    },
    new Argument<string>("version") { 
        Description =  "A version of release",
    },
    new Option<string?>("--url") {
        Description =  "A zip url of release",
    },
    new Option<FileInfo>("--zip") {
        Description =  "A zip path of release",
        IsRequired = false
    },
};

releaseCommand.Handler = CommandHandler.Create(async (
    string package,
    string version,
    string? url,
    FileInfo? zip,
    string? subscription,
    string? resourceGroup,
    string? cosmosdbName,
    string? cosmosdbEndpoint,
    string? storageName,
    string? storageContainerName,
    string? storageContainerEndpoint,
    string? tenantId,
    string? clientId,
    string? clientSecret,
    string? fileStorePath,
    string? fileStoragePath,
    bool? useMachineVault) =>
{
    if (!Regex.IsMatch(package, RegularExpression.PackageName))
        throw new ArgumentException($"{nameof(package)} is invalid pattern ({RegularExpression.PackageName})");

    if (!Regex.IsMatch(version, RegularExpression.ReleaseVersion))
        throw new ArgumentException($"{nameof(version)} is invalid pattern ({RegularExpression.ReleaseVersion})");

    var config = new Config
    {
        SubscriptionId        = fallback(subscription,             envValue("AZURE_SUBSCRIPTION_ID")) ?? throw new ArgumentNullException(nameof(subscription)),
        ResourceGroupName     = fallback(resourceGroup,            envValue("SABURAIIS_RG_NAME"))     ?? throw new ArgumentNullException(nameof(resourceGroup)),
        CosmosDbName          = fallback(cosmosdbName,             envValue("SABURAIIS_DB_NAME")),
        CosmosDbEndpoint      = fallback(cosmosdbEndpoint,         envValue("SABURAIIS_DB_ENDPOINT")),
        StorageAccountName    = fallback(storageName,              envValue("SABURAIIS_STORAGE_NAME")),
        BlobContainerName     = fallback(storageContainerName,     envValue("SABURAIIS_STORAGE_CONTAINER_NAME")),
        BlobContainerEndpoint = fallback(storageContainerEndpoint, envValue("SABURAIIS_STORAGE_ENDPOINT")),
        AADTenantId           = fallback(tenantId,                 envValue("AZURE_TENANT_ID")),
        AADClientId           = fallback(clientId,                 envValue("AZURE_CLIENT_ID")),
        AADClientSecret       = fallback(clientSecret,             envValue("AZURE_CLIENT_SECRET")),
        FileStoreDirectoryPath     = fallback(fileStorePath,       envValue("SABURAIIS_FILE_STORE_DIR_PATH")),
        FileStorageDirectoryPath   = fallback(fileStoragePath,     envValue("SABURAIIS_FILE_STORAGE_DIR_PATH")),
        UseMachineVault = bool.Parse(fallback(useMachineVault?.ToString(), envValue("SABURAIIS_USE_MACHINE_VAULT")) ?? "false"),
    };

    var store = Factory.CreateStore(config);

    await store.InitAsync();

    var (packageData, etag) = await store.GetPackageAsync(package);

    if (packageData == null)
    {
        packageData = new Package { Name = package };
        await store.SavePackageAsync(packageData, etag);
    }

    if (packageData.Releases.Any(r => r.Version == version))
        throw new ArgumentException($"the release version is exist.");

    if (string.IsNullOrEmpty(url) && zip?.Exists != true)
        throw new ArgumentException($"--url or --zip option is needed");

    if (string.IsNullOrEmpty(url))
        url = new Uri(new Uri(config.GetBlobContainerEndpoint()), $"{package.ToLower()}/{version.ToLower()}.zip").AbsoluteUri;

    if (!Regex.IsMatch(url, RegularExpression.ReleaseUrl))
        throw new ArgumentException($"{nameof(url)} is invalid pattern ({RegularExpression.ReleaseUrl})");

    if (zip?.Exists == true)
    {
        var storage = Factory.CreateStorage(config);
        await storage.UploadAsync(url, zip.FullName);
    }
    
    packageData.Releases.Add(new Release
    {
        Version = version,
        Url = url,
        ReleaseAt = DateTimeOffset.Now
    });

    await store.SavePackageAsync(packageData, etag);
});

rootCommand.AddCommand(releaseCommand);

var modifyPathCommand = new Command("modify-path", "Modify site/applications/virtualDirectories/physicalPath the package release.")
{
    new Argument<string>("site") {
        Description = "A name of site",
    },
    new Argument<string>("app") {
        Description =  "A name of application",
    },
    new Argument<string>("package") {
        Description = "A name of package",
    },
    new Argument<string>("version") {
        Description =  "A version of release",
    },
    new Argument<string>("path") {
        Description =  "A JSON file path for partition",
    },
    new Option<string>("location", () => new Config().LocationRoot)
    {
        Description = "Location root path of application physical path",
        IsRequired = false
    }
};

modifyPathCommand.Handler = CommandHandler.Create(async (
    string site,
    string app,
    string package,
    string version,
    FileInfo path,
    string  location,
    string? subscription,
    string? resourceGroup,
    string? cosmosdbName,
    string? cosmosdbEndpoint,
    string? tenantId,
    string? clientId,
    string? clientSecret,
    string? fileStorePath,
    string? fileStoragePath,
    bool? useMachineVault) =>
{
    if (!path.Exists)
        throw new ArgumentException($"{path.FullName} is not exists");

    var json = await File.ReadAllTextAsync(path.FullName);
    var partitionData = JsonSerializer.Deserialize<Partition>(json, jsonOptions);

    if (partitionData == null)
        throw new ArgumentException($"{path.FullName} is invalid for partition");

    var config = new Config
    {
        SubscriptionId    = fallback(subscription,     envValue("AZURE_SUBSCRIPTION_ID")) ?? throw new ArgumentNullException(nameof(subscription)),
        ResourceGroupName = fallback(resourceGroup,    envValue("SABURAIIS_RG_NAME"))     ?? throw new ArgumentNullException(nameof(resourceGroup)),
        CosmosDbName      = fallback(cosmosdbName,     envValue("SABURAIIS_DB_NAME")),
        CosmosDbEndpoint  = fallback(cosmosdbEndpoint, envValue("SABURAIIS_DB_ENDPOINT")),
        AADTenantId       = fallback(tenantId,         envValue("AZURE_TENANT_ID")),
        AADClientId       = fallback(clientId,         envValue("AZURE_CLIENT_ID")),
        AADClientSecret   = fallback(clientSecret,     envValue("AZURE_CLIENT_SECRET")),
        FileStoreDirectoryPath     = fallback(fileStorePath,     envValue("SABURAIIS_FILE_STORE_DIR_PATH")),
        FileStorageDirectoryPath   = fallback(fileStoragePath,   envValue("SABURAIIS_FILE_STORAGE_DIR_PATH")),
        UseMachineVault = bool.Parse(fallback(useMachineVault?.ToString(), envValue("SABURAIIS_USE_MACHINE_VAULT")) ?? "false"),
    };

    var store = Factory.CreateStore(config);

    await store.InitAsync();

    var releaseData = await store.GetReleaseAsync(package, version);

    if (releaseData == null)
        throw new ArgumentException($"The release {package}@{version} is not found");

    var siteData = partitionData.Sites.FirstOrDefault(o => o.Name == site);

    if (siteData == null)
        throw new ArgumentException($"The site {site} is not found");

    var appData = siteData.Applications.FirstOrDefault(o => o.Path == app);

    if (appData == null)
        throw new ArgumentException($"The app {app} is not found");

    var vdir = appData.VirtualDirectories.FirstOrDefault(o => o.Path == "/");

    if (vdir == null)
        throw new ArgumentException($"The virtual directory is not exists in the app ({app})");

    vdir.PhysicalPath = Path.Combine(location, package, version);

    await File.WriteAllTextAsync(path.FullName, JsonSerializer.Serialize(partitionData, jsonOptions));
});

var serveCommand = new Command("serve", "Serve web admin")
{
};

serveCommand.Handler = CommandHandler.Create(async (
    string? subscription,
    string? resourceGroup,
    string? cosmosdbName,
    string? cosmosdbEndpoint,
    string? tenantId,
    string? clientId,
    string? clientSecret,
    string? fileStorePath,
    string? fileStoragePath,
    bool? useMachineVault) =>
{
    var hostBuilder = 
        Host.CreateDefaultBuilder()
            .ConfigureHostConfiguration(hostConfig =>
            {
                hostConfig.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["SaburaIIS:SubscriptionId"]    = subscription,
                    ["SaburaIIS:ResourceGroupName"] = resourceGroup,
                    ["SaburaIIS:CosmosDbName"]      = cosmosdbName,
                    ["SaburaIIS:CosmosDbEndpoint"]  = cosmosdbEndpoint,
                    ["SaburaIIS:AADTenantId"]       = tenantId,
                    ["SaburaIIS:AADClientId"]       = clientId,
                    ["SaburaIIS:AADClientSecret"]   = clientSecret,
                    ["SaburaIIS:FileStorageDirectoryPath"]   = fileStoragePath,
                    ["SaburaIIS:FileStoreDirectoryPath"]     = fileStorePath,
                    ["SaburaIIS:UseMachineVault"]            = useMachineVault?.ToString(),
                });
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });

    await hostBuilder.Build().RunAsync();
});

rootCommand.AddCommand(serveCommand);

return rootCommand.InvokeAsync(args).Result;