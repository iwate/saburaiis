using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SaburaIIS.Agent
{
    public class ServerConfigWatcher
    {
        private const string WATCH_DIR = @"%SystemRoot%\System32\inetsrv\Config";
        private readonly string _path;
        private readonly ILogger _logger;
        private FileSystemWatcher _watcher;
        public ServerConfigWatcher(ILogger logger, string path = WATCH_DIR)
        {
            _logger = logger;
            _path = path;
            _watcher = new FileSystemWatcher();
        }
        public virtual async Task StartWatchAsync(Func<Task> handler)
        {
            await handler();

            _watcher.Path = Environment.ExpandEnvironmentVariables(_path);
            _watcher.Filter = "*.config";
            _watcher.NotifyFilter = NotifyFilters.LastWrite;
            _watcher.IncludeSubdirectories = false;
            _watcher.Changed += async (object sender, FileSystemEventArgs e) => {
                try
                {
                    await handler();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while reporting instance changes");
                }
            };
            _watcher.EnableRaisingEvents = true;
        }
    }
}
