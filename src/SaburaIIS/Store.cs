using Azure.Identity;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Services.AppAuthentication;
using SaburaIIS.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SaburaIIS
{
    public class Store
    {
        private readonly Config _config;
        private CosmosClient _client;
        private Container _partitions;
        private Container _packages;
        private Container _instances;
        private Container _snapshots;
        public Store(Config config)
        {
            _config = config;
        }
        public virtual async Task InitAsync()
        {
            _client = await CreateClient(_config).ConfigureAwait(false);
            var databaseResponse = await _client.CreateDatabaseIfNotExistsAsync(_config.DatabaseName);
            var partitionsResponse = await databaseResponse.Database.CreateContainerIfNotExistsAsync(new ContainerProperties
            {
                Id = $"{_config.ContainerNamePrefix ?? string.Empty}partitions",
                PartitionKeyPath = $"/{nameof(Partition.Name)}",
                DefaultTimeToLive = -1,
                IndexingPolicy = new IndexingPolicy { 
                    IncludedPaths =
                    {
                        new IncludedPath { Path = "/" }
                    },
                    ExcludedPaths =
                    {
                        new ExcludedPath{ Path = $"/{nameof(Partition.ApplicationPools)}/*"},
                        new ExcludedPath{ Path = $"/{nameof(Partition.Sites)}/*"},
                    }
                }
            });
            _partitions = partitionsResponse.Container;

            var packagesResponse = await databaseResponse.Database.CreateContainerIfNotExistsAsync(new ContainerProperties
            {
                Id = $"{_config.ContainerNamePrefix ?? string.Empty}packages",
                PartitionKeyPath = $"/{nameof(Package.Name)}",
                DefaultTimeToLive = -1,
            });
            _packages = packagesResponse.Container;

            var instancesResponse = await databaseResponse.Database.CreateContainerIfNotExistsAsync(new ContainerProperties
            {
                Id = $"{_config.ContainerNamePrefix ?? string.Empty}instances",
                PartitionKeyPath = $"/{nameof(VirtualMachine.ScaleSetName)}",
                DefaultTimeToLive = -1,
            });
            _instances = instancesResponse.Container;

            var snapshotsResponse = await databaseResponse.Database.CreateContainerIfNotExistsAsync(new ContainerProperties
            {
                Id = $"{_config.ContainerNamePrefix ?? string.Empty}snapshots",
                PartitionKeyPath = $"/{nameof(Snapshot.Name)}",
                DefaultTimeToLive = -1,
                IndexingPolicy = new IndexingPolicy
                {
                    IncludedPaths =
                    {
                        new IncludedPath { Path = "/" }
                    },
                    ExcludedPaths =
                    {
                        new ExcludedPath{ Path = $"/{nameof(Snapshot.ApplicationPools)}/*"},
                        new ExcludedPath{ Path = $"/{nameof(Snapshot.Sites)}/*"},
                    }
                }
            });
            _snapshots = snapshotsResponse.Container;
        }

        public virtual async Task<string> SavePartitionAsync(Partition partition, string etag)
        {
            var response = await _partitions.UpsertItemAsync(partition, new PartitionKey(partition.Name), new ItemRequestOptions { IfMatchEtag = etag });
            return response.Headers.ETag;
        }

        public virtual async Task RemovePartitionAsync(Partition partition, string etag)
        {
            await _partitions.DeleteItemAsync<Partition>(partition.id, new PartitionKey(partition.Name), new ItemRequestOptions { IfMatchEtag = etag });
        }

        public virtual async Task<string> SearchPartitionAsync(string vmss)
        {
            var query = new QueryDefinition("SELECT VALUE(T.Name) FROM T JOIN ss IN T.ScaleSets WHERE ss.Name = @vmss").WithParameter("@vmss", vmss);
            using var feedIterator = _partitions.GetItemQueryIterator<string>(query);
            while (feedIterator.HasMoreResults)
            {
                var response = await feedIterator.ReadNextAsync();
                foreach (var partition in response)
                    return partition;
            }
            return null;
        }
        public virtual async Task<IEnumerable<string>> GetPartitionNamesAsync()
        {
            var result = new List<string>();
            using var feedIterator = _partitions.GetItemQueryIterator<Partition>("SELECT T.Name FROM T");
            while (feedIterator.HasMoreResults)
            {
                var response = await feedIterator.ReadNextAsync();
                foreach (var partition in response)
                    result.Add(partition.Name);
            }
            return result;
        }
        public virtual async Task<(Partition, string)> GetPartitionAsync(string name)
        {
            var response = await _partitions.ReadItemAsync<Partition>(name, new PartitionKey(name));
            return (response.Resource, response.ETag);
        }

        public virtual async Task SavePackageAsync(Package package, string etag)
        {
            await _packages.UpsertItemAsync(package, new PartitionKey(package.Name), new ItemRequestOptions { IfMatchEtag = etag });
        }

        public virtual async Task<IEnumerable<string>> GetPackageNamesAsync()
        {
            var result = new List<string>();
            using var feedIterator = _packages.GetItemQueryIterator<Package>("SELECT T.Name FROM T");
            while (feedIterator.HasMoreResults)
            {
                var response = await feedIterator.ReadNextAsync();
                foreach (var package in response)
                    result.Add(package.Name);
            }
            return result;
        }

        public virtual async Task<(Package, string)> GetPackageAsync(string name)
        {
            try
            {
                var response = await _packages.ReadItemAsync<Package>(name, new PartitionKey(name));
                return (response.Resource, response.ETag);
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return (null, null);
            }
        }

        public virtual async Task<IEnumerable<string>> GetReleaseVersionsAsync(string packageName)
        {
            var result = new List<string>();
            var query = new QueryDefinition("SELECT r.Version FROM T JOIN r IN T.Releases WHERE T.Name = @name").WithParameter("@name", packageName);
            using var feedIterator = _packages.GetItemQueryIterator<Release>(query);
            while (feedIterator.HasMoreResults)
            {
                var response = await feedIterator.ReadNextAsync();
                foreach (var release in response)
                    result.Add(release.Version);
            }
            return result;
        }

        public virtual async Task<Release> GetReleaseAsync(string packageName, string version)
        {
            var query = new QueryDefinition("SELECT VALUE(r) FROM T JOIN r IN T.Releases WHERE T.Name = @name AND r.Version = @version")
                                .WithParameter("@name", packageName)
                                .WithParameter("@version", version);
            using var feedIterator = _packages.GetItemQueryIterator<Release>(query);
            while (feedIterator.HasMoreResults)
            {
                var response = await feedIterator.ReadNextAsync();
                foreach (var release in response)
                    return release;
            }
            return null;
        }

        public virtual async Task SaveInstanceAsync(VirtualMachine vm)
        {
            await _instances.UpsertItemAsync(vm, new PartitionKey(vm.ScaleSetName));
        }

        public virtual async Task<IEnumerable<VirtualMachine>> GetInstancesAsync(string vmss, string etag = null)
        {
            var result = new List<VirtualMachine>();
            var query = string.IsNullOrEmpty(etag)
                      ? new QueryDefinition("SELECT VALUE(T) FROM T WHERE T.ScaleSetName = @vmss").WithParameter("@vmss", vmss)
                      : new QueryDefinition("SELECT VALUE(T) FROM T WHERE T.ScaleSetName = @vmss AND T.PartitionETag = @etag").WithParameter("@vmss", vmss).WithParameter("@etag", etag);
            using var feedIterator = _instances.GetItemQueryIterator<VirtualMachine>(query);
            while (feedIterator.HasMoreResults)
            {
                var response = await feedIterator.ReadNextAsync();
                foreach (var vm in response)
                    result.Add(vm);
            }
            return result;
        }

        public virtual async Task<VirtualMachine> GetInstanceAsync(string vmss, string name)
        {
            var result = new List<VirtualMachine>();
            var query = new QueryDefinition("SELECT VALUE(T) FROM T WHERE T.ScaleSetName = @vmss AND T.Name = @name")
                            .WithParameter("@vmss", vmss)
                            .WithParameter("@name", name);
            using var feedIterator = _instances.GetItemQueryIterator<VirtualMachine>(query);
            while (feedIterator.HasMoreResults)
            {
                var response = await feedIterator.ReadNextAsync();
                foreach (var vm in response)
                    return vm;
            }

            return null;
        }

        public virtual async Task AddSnapshoptAsync(Snapshot snapshot)
        {
            await _snapshots.UpsertItemAsync(snapshot, new PartitionKey(snapshot.Name));
        }

        public virtual async Task<IEnumerable<DateTimeOffset>> GetSnapshotsAsync(string scaleSet, string name)
        {
            var result = new List<DateTimeOffset>();
            var query = new QueryDefinition("SELECT VALUE(T.Timestamp) FROM T WHERE T.ScaleSetName = @scaleSet AND T.Name = @name ORDER BY T.Timestamp DESC")
                .WithParameter("@scaleSet", scaleSet)
                .WithParameter("@name", name);
            using var feedIterator = _snapshots.GetItemQueryIterator<DateTimeOffset>(query);
            while (feedIterator.HasMoreResults)
            {
                var response = await feedIterator.ReadNextAsync();
                foreach (var snapshot in response)
                    result.Add(snapshot);
            }
            return result;
        }

        public virtual async Task<Snapshot> GetSnapshotAsync(string scaleSet, string name, string timestamp)
        {
            var query = new QueryDefinition("SELECT VALUE(T) FROM T WHERE T.ScaleSetName = @scaleSet AND T.Name = @name AND T.Timestamp = @timestamp")
                .WithParameter("@scaleSet", scaleSet)
                .WithParameter("@name", name)
                .WithParameter("@timestamp", timestamp);
            using var feedIterator = _snapshots.GetItemQueryIterator<Snapshot>(query);
            while (feedIterator.HasMoreResults)
            {
                var response = await feedIterator.ReadNextAsync();
                foreach (var snapshot in response)
                    return snapshot;
            }
            return null;
        }

        public virtual ChangeTracker<Partition> CreatePartitionChangeTracker(string partitionName)
        {
            var feedRange = FeedRange.FromPartitionKey(new PartitionKey(partitionName));
            var startAt = ChangeFeedStartFrom.Now(feedRange);
            var feed = _partitions.GetChangeFeedIterator<Partition>(startAt, ChangeFeedMode.Incremental);

            return new ChangeTracker<Partition>(feed);
        }

        public virtual ChangeTracker<VirtualMachine> CreateInstancesChangeTracker(string scaleSetName)
        {
            var feedRange = FeedRange.FromPartitionKey(new PartitionKey(scaleSetName));
            var startAt = ChangeFeedStartFrom.Now(feedRange);
            var feed = _instances.GetChangeFeedIterator<VirtualMachine>(startAt, ChangeFeedMode.Incremental);

            return new ChangeTracker<VirtualMachine>(feed);
        }
        private const string TOKEN_ENDPOINT = "https://management.azure.com/";
        private const string KEYS_ENDPOINT = "https://management.azure.com/subscriptions/{0}/resourceGroups/{1}/providers/Microsoft.DocumentDB/databaseAccounts/{2}/listKeys?api-version=2019-12-12";
        private static readonly HttpClient httpClient = new HttpClient();
        private static async ValueTask<CosmosClient> CreateClientByManagedIdentity(Config config)
        {
            var azureServiceTokenProvider = new AzureServiceTokenProvider();

            var accessToken = await azureServiceTokenProvider.GetAccessTokenAsync(TOKEN_ENDPOINT);

            var endpoint = string.Format(KEYS_ENDPOINT, config.SubscriptionId, config.ResourceGroupName, config.CosmosDbName);

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var result = await httpClient.PostAsync(endpoint, new StringContent(""));

            dynamic keys = await System.Text.Json.JsonSerializer.DeserializeAsync<System.Dynamic.ExpandoObject> (await result.Content.ReadAsStreamAsync());

            return new CosmosClient(config.GetCosmosDbEndpoint(), keys.primaryMasterKey.ToString());
        }

        private static CosmosClient CreateClientByRBAC(Config config)
        {
            var servicePrincipal = new ClientSecretCredential(config.AADTenantId,config.AADClientId,config.AADClientSecret);
            return new CosmosClient(config.GetCosmosDbEndpoint(), servicePrincipal);
        }

        private static async ValueTask<CosmosClient> CreateClient(Config config)
        {
            if (!string.IsNullOrEmpty(config.AADClientSecret))
                return CreateClientByRBAC(config);

            return await CreateClientByManagedIdentity(config);
        }
    }
}
