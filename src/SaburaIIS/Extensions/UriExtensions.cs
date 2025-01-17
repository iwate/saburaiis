using System;

namespace SaburaIIS.Extensions
{
    public static class UriExtensions
    {
        public static string ToPath(this Uri uri)
        {
            if (uri.Scheme != "file")
            {
                throw new ArgumentException("The uri should be start 'file' schema");
            }

            var path = uri.AbsolutePath.Replace("/", @"\");

            if (string.IsNullOrEmpty(uri.Host))
            {
                return path;
            }

            return @"\\" + uri.Host + path;
        }
    }
}
