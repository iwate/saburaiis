using System.Collections.Generic;
using System.IO;

namespace SaburaIIS.Agent.Extensions
{
    public static class DirectoryInfoExtensions
    {
        public static void CreateRecursive(this DirectoryInfo? info)
        {
            var dirs = new Stack<string>();
            while (info != null && !info.Exists)
            {
                dirs.Push(info.FullName);
                info = info.Parent;
            }
            while (dirs.TryPop(out var path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }
}
