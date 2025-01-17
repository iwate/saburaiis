using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SaburaIIS
{
    public interface IStorage
    {
        bool CanDownload(string url);
        Task<Stream> DownloadAsync(string url);

        bool CanUpload(string url);
        Task UploadAsync(string url, string path);
    }

    public class Storage : IStorage
    {
        private readonly IStorage[] _storages;

        public Storage(IStorage[] storages)
        {
            _storages = storages ?? throw new ArgumentNullException();
        }

        public bool CanDownload(string url) => throw new NotImplementedException();

        public virtual async Task<Stream> DownloadAsync(string url)
        {
            var storage = _storages.FirstOrDefault(s => s.CanDownload(url)) ?? throw new NotSupportedException();
            return await storage.DownloadAsync(url);
        }

        public bool CanUpload(string url) => throw new NotImplementedException();

        public virtual async Task UploadAsync(string url, string path)
        {
            var storage = _storages.FirstOrDefault(s => s.CanUpload(url)) ?? throw new NotSupportedException();
            await storage.UploadAsync(url, path);
        }
    }
}
