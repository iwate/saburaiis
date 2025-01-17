namespace SaburaIIS
{
    public class Config
    {
        public string SubscriptionId { get; set; }
        public string ResourceGroupName { get; set; }
        private string _cosmosDbName;
        public string CosmosDbName
        {
            get => string.IsNullOrEmpty(_cosmosDbName) ? ResourceGroupName : _cosmosDbName;
            set => _cosmosDbName = value;
        }
        public string CosmosDbEndpoint { get; set; }
        private string _keyVaultName;
        public string KeyVaultName
        {
            get => string.IsNullOrEmpty(_keyVaultName) ? ResourceGroupName : _keyVaultName;
            set => _keyVaultName = value;
        }
        public string KeyVaultEndpoint { get; set; }
        private string? _appConfigName;
        public string AppConfigurationName
        {
            get => string.IsNullOrEmpty(_appConfigName) ? ResourceGroupName : _appConfigName;
            set => _appConfigName = value;
        }
        public string? AppConfigurationEndpoint { get; set; }
        private string _storageAccountName;
        public string StorageAccountName
        {
            get => string.IsNullOrEmpty(_storageAccountName) ? ResourceGroupName : _storageAccountName;
            set => _storageAccountName = value;
        }
        public string AzureBlobStorageHost { get; set; } = ".blob.core.windows.net";
        public string BlobContainerName { get; set; } = "packages";
        public string BlobContainerEndpoint { get; set; }
        public string AADTenantId { get; set; }
        public string AADClientId { get; set; }
        public string AADClientSecret { get; set; }

        public string DatabaseName { get; set; } = "saburaiis";
        public string ContainerNamePrefix { get; set; } = "";
        public string LocationRoot { get; set; } = @"%SystemDrive%\inetpub\sites";
        public string AppLocationDefault { get; set; } = @"%SystemDrive%\inetpub\wwwroot";
        public string LogLocationDefault { get; set; } = @"%SystemDrive%\inetpub\logs\LogFiles";

        public string GetCosmosDbEndpoint() => CosmosDbEndpoint ?? $"https://{CosmosDbName}.documents.azure.com/";
        public string GetKeyVaultEndpoint() => KeyVaultEndpoint ?? $"https://{KeyVaultName}.vault.azure.net/";
        public string GetBlobContainerEndpoint() => BlobContainerEndpoint ?? $"https://{StorageAccountName}.blob.core.windows.net/{BlobContainerName}/";
        public string GetAppConfigurationEndpoint() => AppConfigurationEndpoint ?? $"https://{AppConfigurationName}.azconfig.io";

        public string FileStorageDirectoryPath { get; set; } = null;
        public string FileStoreDirectoryPath { get; set; } = null;
        public string FileVariablesDirectoryPath { get; set; } = null;
        public bool UseMachineVault { get; set; } = false;
    }
}
