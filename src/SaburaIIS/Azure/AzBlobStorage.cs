using Azure.Core;
using Azure.Identity;
using Azure.Storage.Blobs;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SaburaIIS.Azure
{
    class AzBlobStorage : IStorage
    {
        private readonly Config _config;

        public AzBlobStorage(Config config)
        {
            _config = config;
        }

        private bool IsAzureBlob(string url)
        {
            var uri = new Uri(url);
            return uri.Host.EndsWith(_config.AzureBlobStorageHost) && string.IsNullOrEmpty(uri.Query);
        }

        public bool CanDownload(string url) => IsAzureBlob(url);

        public async Task<Stream> DownloadAsync(string url)
        {
            var blobClient = new BlobClient(new Uri(url), CreateTokenCredential());
            var streaming = await blobClient.DownloadStreamingAsync();
            return streaming.Value.Content;
        }

        public bool CanUpload(string url) => IsAzureBlob(url);
        
        public async Task UploadAsync(string url, string path)
        {
            var blobClient = new BlobClient(new Uri(url), CreateTokenCredential());
            using var file = File.OpenRead(path);
            await blobClient.UploadAsync(file);
        }

        private TokenCredential CreateTokenCredential()
        {
            return !string.IsNullOrEmpty(_config.AADClientSecret)
                        ? new ClientSecretCredential(_config.AADTenantId, _config.AADClientId, _config.AADClientSecret)
                        : new DefaultAzureCredential();
        }
    }
}
