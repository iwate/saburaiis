using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace SaburaIIS.Remote
{
    class PublicHttpStorage : IStorage
    {
        private readonly HttpClient _httpClient;

        public PublicHttpStorage(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public bool CanDownload(string url)
        {
            return true;
        }

        public async Task<Stream> DownloadAsync(string url)
        {
            return await _httpClient.GetStreamAsync(url);
        }

        public bool CanUpload(string url)
        {
            return false;
        }

        public Task UploadAsync(string url, string path)
        {
            throw new NotSupportedException();
        }
    }
}
