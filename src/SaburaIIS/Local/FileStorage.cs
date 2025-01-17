using System;
using System.IO;
using System.Threading.Tasks;
using SaburaIIS.Extensions;

namespace SaburaIIS.Local
{
    public class FileStorage : IStorage
    {
        public FileStorage(Config config) 
        {
        }

        public bool CanDownload(string url)
        {
            try
            {
                var uri = new Uri(url);
                if (uri.Scheme != "file")
                {
                    return false;
                }

                return File.Exists(uri.ToPath());
            }
            catch
            {
                return false;
            }
        }

        public bool CanUpload(string url)
        {
            try
            {
                var uri = new Uri(url);
                if (uri.Scheme != "file")
                {
                    return false;
                }

                return Directory.Exists(new FileInfo(uri.ToPath()).DirectoryName);
            }
            catch
            {
                return false;
            }
        }

        public Task<Stream> DownloadAsync(string url)
        {
            return Task.FromResult<Stream>(File.OpenRead(new Uri(url).ToPath()));
        }

        public async Task UploadAsync(string url, string path)
        {
            using var src = File.OpenRead(path);
            using var dst = File.OpenWrite(new Uri(url).ToPath());

            await src.CopyToAsync(dst);
        }

    }
}
