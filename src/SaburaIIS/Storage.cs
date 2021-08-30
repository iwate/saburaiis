using Azure.Core;
using Azure.Identity;
using Azure.Storage.Blobs;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace SaburaIIS
{
    public class Storage
    {
        private readonly Config _config;

        public Storage(Config config)
        {
            _config = config;
        }

        private readonly static HttpClient _httpClient = new HttpClient();

        private bool IsAzureBlog(string url)
        {
            var uri = new Uri(url);
            return uri.Host.EndsWith("blob.core.windows.net") && string.IsNullOrEmpty(uri.Query);
        }
        public virtual async Task<Stream> DownloadAsync(string url)
        {
            if (IsAzureBlog(url))
                return await DownloadAzureBlobAsync(url);

            else
                return await DownloadPublicHttpAsync(url);
        }
        private async Task<Stream> DownloadAzureBlobAsync(string url)
        {
            var blobClient = new BlobClient(new Uri(url), CreateTokenCredential());
            var streaming = await blobClient.DownloadStreamingAsync();
            return streaming.Value.Content;
        }

        private async Task<Stream> DownloadPublicHttpAsync(string url)
        {
            return await _httpClient.GetStreamAsync(url);
        }

        public virtual async Task UploadAsync(string url, string path)
        {
            if (!IsAzureBlog(url))
                throw new NotSupportedException();

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
