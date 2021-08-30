using SaburaIIS;
using SaburaIIS.Json;
using SaburaIIS.Models;
using SaburaIIS.Validations;
using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

var rootCommand = new RootCommand("SaburaIIS command-line tool");

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
rootCommand.AddGlobalOption(new Option<string?>("--cosmosdb")
{ 
    Description= "CosmosDB name (fallback to SABURAIIS_DB_NAME environment variable)",
    IsRequired = false
});
rootCommand.AddGlobalOption(new Option<string?>("--cosmosdb-endpoint") 
{
    Description = "CosmosDB endpoint (fallback to SABURAIIS_DB_ENDPOINT environment variable)",
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

var jsonOptions = new JsonSerializerOptions
{
    WriteIndented = true,
};
jsonOptions.Converters.Add(new JsonStringEnumConverter());
jsonOptions.Converters.Add(new TimeSpanConverter());

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
    string? cosmosdb,
    string? cosmosdbEndpoint,
    string? tenantId,
    string? clientId,
    string? clientSecret) =>
{
    var env = Environment.GetEnvironmentVariables();
    string? envValue(string name) => env.Contains(name) ? (string?)env[name] : null;
    string? fallback(params string?[] values) => values.FirstOrDefault(value => !string.IsNullOrEmpty(value));

    var config = new Config
    {
        SubscriptionId    = fallback(subscription,     envValue("AZURE_SUBSCRIPTION_ID")) ?? throw new ArgumentNullException(nameof(subscription)),
        ResourceGroupName = fallback(resourceGroup,    envValue("SABURAIIS_RG_NAME"))     ?? throw new ArgumentNullException(nameof(resourceGroup)),
        CosmosDbName      = fallback(cosmosdb,         envValue("SABURAIIS_DB_NAME"))     ?? throw new ArgumentNullException(nameof(cosmosdb)),
        CosmosDbEndpoint  = fallback(cosmosdbEndpoint, envValue("SABURAIIS_DB_ENDPOINT")),
        AADTenantId       = fallback(tenantId,         envValue("AZURE_TENANT_ID")),
        AADClientId       = fallback(clientId,         envValue("AZURE_CLIENT_ID")),
        AADClientSecret   = fallback(clientSecret,     envValue("AZURE_CLIENT_SECRET")),
    };

    var store = new Store(config);

    await store.InitAsync();

    var (data, etag) = await store.GetPartitionAsync(partition);

    if (data == null)
    {
        Console.WriteLine($"{partition} is not found.");
        return;
    }

    var json = JsonSerializer.Serialize(data, jsonOptions);

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
    string? cosmosdb,
    string? cosmosdbEndpoint,
    string? tenantId,
    string? clientId,
    string? clientSecret) =>
{
    if (!input.Exists)
        throw new ArgumentException($"{nameof(input.FullName)} is not exists");

    var env = Environment.GetEnvironmentVariables();
    string? envValue(string name) => env.Contains(name) ? (string?)env[name] : null;
    string? fallback(params string?[] values) => values.FirstOrDefault(value => !string.IsNullOrEmpty(value));

    var config = new Config
    {
        SubscriptionId    = fallback(subscription,     envValue("AZURE_SUBSCRIPTION_ID")) ?? throw new ArgumentNullException(nameof(subscription)),
        ResourceGroupName = fallback(resourceGroup,    envValue("SABURAIIS_RG_NAME"))     ?? throw new ArgumentNullException(nameof(resourceGroup)),
        CosmosDbName      = fallback(cosmosdb,         envValue("SABURAIIS_DB_NAME"))     ?? throw new ArgumentNullException(nameof(cosmosdb)),
        CosmosDbEndpoint  = fallback(cosmosdbEndpoint, envValue("SABURAIIS_DB_ENDPOINT")),
        AADTenantId       = fallback(tenantId,         envValue("AZURE_TENANT_ID")),
        AADClientId       = fallback(clientId,         envValue("AZURE_CLIENT_ID")),
        AADClientSecret   = fallback(clientSecret,     envValue("AZURE_CLIENT_SECRET")),
    };

    var store = new Store(config);

    await store.InitAsync();

    var (current, etag) = await store.GetPartitionAsync(partition);

    if (current == null)
    {
        Console.WriteLine($"{partition} is not found.");
        return;
    }

    var json = await File.ReadAllTextAsync(input.FullName);
    var @new = JsonSerializer.Deserialize<Partition>(json, jsonOptions);

    if (@new == null)
        throw new ArgumentException($"{input.Name} is invalid partition json file");

    if (@new.Name != partition)
        throw new ArgumentException($"{input.Name} is not for {partition}");

    await store.SavePartitionAsync(@new, etag);
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
    new Argument<string?>("url") {
        Description =  "A zip url of release",
    },
    new Option<FileInfo>("zip") {
        Description =  "A zip path of release",
        IsRequired = false
    },
};

releaseCommand.Handler = CommandHandler.Create(async (
    string package,
    string version,
    string url,
    FileInfo zip,
    string? subscription,
    string? resourceGroup,
    string? cosmosdb,
    string? cosmosdbEndpoint,
    string? tenantId,
    string? clientId,
    string? clientSecret) =>
{
    if (!Regex.IsMatch(package, RegularExpression.PackageName))
        throw new ArgumentException($"{nameof(package)} is invalid pattern ({RegularExpression.PackageName})");

    if (!Regex.IsMatch(version, RegularExpression.ReleaseVersion))
        throw new ArgumentException($"{nameof(version)} is invalid pattern ({RegularExpression.ReleaseVersion})");

    if (!Regex.IsMatch(url, RegularExpression.ReleaseUrl))
        throw new ArgumentException($"{nameof(url)} is invalid pattern ({RegularExpression.ReleaseUrl})");

    var env = Environment.GetEnvironmentVariables();
    string? envValue(string name) => env.Contains(name) ? (string?)env[name] : null;
    string? fallback(params string?[] values) => values.FirstOrDefault(value => !string.IsNullOrEmpty(value));

    var config = new Config
    {
        SubscriptionId    = fallback(subscription,     envValue("AZURE_SUBSCRIPTION_ID")) ?? throw new ArgumentNullException(nameof(subscription)),
        ResourceGroupName = fallback(resourceGroup,    envValue("SABURAIIS_RG_NAME"))     ?? throw new ArgumentNullException(nameof(resourceGroup)),
        CosmosDbName      = fallback(cosmosdb,         envValue("SABURAIIS_DB_NAME"))     ?? throw new ArgumentNullException(nameof(cosmosdb)),
        CosmosDbEndpoint  = fallback(cosmosdbEndpoint, envValue("SABURAIIS_DB_ENDPOINT")),
        AADTenantId       = fallback(tenantId,         envValue("AZURE_TENANT_ID")),
        AADClientId       = fallback(clientId,         envValue("AZURE_CLIENT_ID")),
        AADClientSecret   = fallback(clientSecret,     envValue("AZURE_CLIENT_SECRET")),
    };

    var store = new Store(config);

    await store.InitAsync();

    var (packageData, etag) = await store.GetPackageAsync(package);

    if (packageData == null) {
        packageData = new Package { Name = package };
        await store.SavePackageAsync(packageData, etag);
    }

    if (zip.Exists)
    {
        var storage = new Storage(config);
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
    string? cosmosdb,
    string? cosmosdbEndpoint,
    string? tenantId,
    string? clientId,
    string? clientSecret) =>
{
    if (!path.Exists)
        throw new ArgumentException($"{path.FullName} is not exists");

    var json = await File.ReadAllTextAsync(path.FullName);
    var partitionData = JsonSerializer.Deserialize<Partition>(json, jsonOptions);

    if (partitionData == null)
        throw new ArgumentException($"{path.FullName} is invalid for partition");

    var env = Environment.GetEnvironmentVariables();
    string? envValue(string name) => env.Contains(name) ? (string?)env[name] : null;
    string? fallback(params string?[] values) => values.FirstOrDefault(value => !string.IsNullOrEmpty(value));

    var config = new Config
    {
        SubscriptionId    = fallback(subscription,     envValue("AZURE_SUBSCRIPTION_ID")) ?? throw new ArgumentNullException(nameof(subscription)),
        ResourceGroupName = fallback(resourceGroup,    envValue("SABURAIIS_RG_NAME"))     ?? throw new ArgumentNullException(nameof(resourceGroup)),
        CosmosDbName      = fallback(cosmosdb,         envValue("SABURAIIS_DB_NAME"))     ?? throw new ArgumentNullException(nameof(cosmosdb)),
        CosmosDbEndpoint  = fallback(cosmosdbEndpoint, envValue("SABURAIIS_DB_ENDPOINT")),
        AADTenantId       = fallback(tenantId,         envValue("AZURE_TENANT_ID")),
        AADClientId       = fallback(clientId,         envValue("AZURE_CLIENT_ID")),
        AADClientSecret   = fallback(clientSecret,     envValue("AZURE_CLIENT_SECRET")),
    };

    var store = new Store(config);

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

rootCommand.AddCommand(modifyPathCommand);

return rootCommand.InvokeAsync(args).Result;