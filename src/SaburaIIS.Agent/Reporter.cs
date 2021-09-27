using Microsoft.Extensions.Logging;
using Microsoft.Web.Administration;
using SaburaIIS.Agent.Mappers;
using SaburaIIS.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SaburaIIS.Agent
{
    public class Reporter
    {
        private const string WATCH_DIR = @"%SystemRoot%\System32\inetsrv\Config";
        private readonly string _path;
        private readonly string _scaleSetName;
        private readonly Store _store;
        private readonly ILogger _logger;
        private FileSystemWatcher _watcher;
        private Mapper _mapper;
        public Reporter(string scaleSetName, Store store, ILogger logger, string path = WATCH_DIR)
        {
            _scaleSetName = scaleSetName;
            _store = store;
            _logger = logger;
            _path = path;
            _watcher = new FileSystemWatcher();
            _mapper = new Mapper();
        }
        public virtual async Task StartWatchAsync()
        {
            await ReportAsync();

            _watcher.Path = Environment.ExpandEnvironmentVariables(_path);
            _watcher.Filter = "*.config";
            _watcher.NotifyFilter = NotifyFilters.LastWrite;
            _watcher.IncludeSubdirectories = false;
            _watcher.Changed += async (object sender, FileSystemEventArgs e) => {
                try
                {
                    await ReportAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while reporting instance changes");
                }
            };
            _watcher.EnableRaisingEvents = true;
        }

        public virtual async Task ReportAsync()
        {
            var manager = new ServerManager();
            var applicationPools = _mapper.Map<ApplicationPool, POCO.ApplicationPool>(manager.ApplicationPools);
            var sites = _mapper.Map<Site, POCO.Site>(manager.Sites);

            await _store.SaveInstanceAsync(new VirtualMachine
            {
                ScaleSetName = _scaleSetName,
                Name = Environment.MachineName,
                Current = new Snapshot
                {
                    ScaleSetName = _scaleSetName,
                    Name = Environment.MachineName,
                    Timestamp = DateTimeOffset.Now,
                    ApplicationPools = applicationPools.ToList(),
                    Sites = sites.ToList()
                }
            });
        }
    }
}
