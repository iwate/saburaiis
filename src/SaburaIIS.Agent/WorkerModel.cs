using Microsoft.Extensions.Logging;
using Microsoft.Web.Administration;
using SaburaIIS.Agent.Extensions;
using SaburaIIS.Agent.Mappers;
using SaburaIIS.Agent.Transformers;
using SaburaIIS.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SaburaIIS.Agent
{
    public class WorkerModel
    {
        private readonly Config _config;
        private readonly ILogger<WorkerModel> _logger;
        private readonly Store _store;
        private readonly Storage _storage;
        private readonly KeyVault _keyVault;
        private readonly AppConfiguration _appConfig;
        private readonly CertificateStoreFactory _certificateStoreFactory;
        private readonly Mapper _mapper;
        private readonly ServerConfigWatcher _watcher;
        private ServerManager _manager;
        private ChangeTracker<Partition>? _tracker;
        private IDictionary<string, DateTimeOffset> _lastRecycleAt;
        private string _etag = string.Empty;
        private static object @lock = new object();

        public WorkerModel(
            Config config,
            Store store,
            Storage storage,
            KeyVault keyVault,
            AppConfiguration appConfig,
            CertificateStoreFactory certificateStoreFactory,
            Mapper mapper,
            ServerConfigWatcher watcher,
            ILogger<WorkerModel> logger
        )
        {
            _config = config;
            _logger = logger;
            _store = store;
            _storage = storage;
            _keyVault = keyVault;
            _appConfig = appConfig;
            _certificateStoreFactory = certificateStoreFactory;
            _mapper = mapper;
            _watcher = watcher;
            _manager = new ServerManager();
            _lastRecycleAt = new Dictionary<string, DateTimeOffset>();
        }
        public async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (true)
            {
                if (stoppingToken.IsCancellationRequested)
                    return;
                try
                {
                    await _store.InitAsync();
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while initializing store.");
                    await Task.Delay(TimeSpan.FromSeconds(_config.PoolingDelaySecForInit), stoppingToken);
                }
            }

            try
            {
                await _watcher.StartWatchAsync(Report);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while starting reporting");
                return;
            }


            while (!stoppingToken.IsCancellationRequested)
            {
                string? partitionName = null;
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        partitionName = await _store.SearchPartitionAsync(_config.ScaleSetName);

                        if (partitionName != null)
                        {
                            ResetTracker(partitionName);
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "An error occurred while waiting assign to partition.");
                        await Task.Delay(TimeSpan.FromSeconds(_config.PoolingDelaySecForAssign), stoppingToken);
                    }
                }

                var changed = true;
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        if (!changed && !(changed = await _tracker!.HasChangeAsync()))
                            continue;

                        var (partition, etag) = await _store.GetPartitionAsync(partitionName);

                        if (partition == null || !partition.ScaleSets.Any(ss => ss.Name == _config.ScaleSetName))
                            break;

                        _etag = etag;

                        await Update(partition);

                        changed = false;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "An error occurred while work.");
                        ResetManager();
                    }
                    finally
                    {
                        await Task.Delay(TimeSpan.FromSeconds(_config.PoolingDelaySecForUpdate), stoppingToken);
                    }
                }
            }
        }
        public virtual async Task Update(Partition partition)
        {
            _logger.LogInformation("Check {partition} changes", partition.Name);

            var applicationPools = _mapper.Map<ApplicationPool, POCO.ApplicationPool>(_manager.ApplicationPools).ToList();
            var sites = _mapper.Map<Site, POCO.Site>(_manager.Sites).ToList();

            var dAppPools = Delta.CreateCollection(applicationPools, partition.ApplicationPools).ToList();
            var dSites = Delta.CreateCollection(sites, partition.Sites).ToList();

            var apppoolNames = dSites.GetApplicationPoolNames(sites).Concat(RecycleApplicationPools(partition)).Distinct();

            if (dAppPools.Count == 0 && dSites.Count == 0 && apppoolNames.Count() == 0)
                return;

            _logger.LogInformation("Detect {partition} changes", partition.Name);

            await DeployCertificates(partition);

            await DeployPackages(dSites);

            _logger.LogInformation("Save snapshot");

            await _store.AddSnapshoptAsync(new Snapshot
            {
                ScaleSetName = _config.ScaleSetName,
                Name = Environment.MachineName,
                Timestamp = DateTimeOffset.Now,
                ApplicationPools = applicationPools,
                Sites = sites
            });

            _logger.LogInformation("Transform application pools");

            foreach (var delta in dAppPools)
                Transformer.Transform(_manager.ApplicationPools, delta);

            _logger.LogInformation("Transform sites");

            foreach (var delta in dSites)
                Transformer.Transform(_manager.Sites, delta);

            _logger.LogInformation("Update environment variables of application pools. ({pools})", apppoolNames);

            await new EnvironmentVariablesWriter(_manager).WriteAsync(apppoolNames, apppoolName => _appConfig.GetConfiguration(partition.Name, apppoolName));

            _logger.LogInformation("Commit changes");

            Console.WriteLine($"CommitChange {Thread.CurrentThread.ManagedThreadId}");
            _manager.CommitChanges();

            _logger.LogInformation("Rycycle application pools");

            RecycleAppPools(apppoolNames);

            SaveLastRecycleAt(partition);

            DeleteOldDires(dSites);
        }

        void ResetManager()
        {
            _manager?.Dispose();
            _manager = new ServerManager();
        }

        void ResetTracker(string partitionName)
        {
            _tracker?.Dispose();
            _tracker = _store.CreatePartitionChangeTracker(partitionName);
        }

        public void RecycleAppPools(IEnumerable<string> applicationPoolNames)
        {
            foreach (var apppoolName in applicationPoolNames)
            {
                _logger.LogInformation("Recycle '{apppoolName}'", apppoolName);
                _manager.ApplicationPools.First(apppool => apppool.Name == apppoolName).Recycle();
            }
        }

        public async Task DeployCertificates(Partition partition)
        {
            var certs = partition.GetCertificates();

            if (!certs.Any())
                return;

            var registry = await _keyVault.GetCertificatesAsync();
            foreach (var group in certs)
            {
                using var store = _certificateStoreFactory.Create(group.Key);
                var notStoredCerts = group.Where(cert => !store.Contains(cert.thumbprint));
                foreach (var cert in notStoredCerts)
                {
                    var info = registry.Find(cert.thumbprint);
                    var raw = await _keyVault.GetCertificateAsync(info.Name, info.Version);
                    if (raw != null)
                    {
                        store.Add(raw);
                    }
                }
            }
        }

        public async Task DeployPackages(IEnumerable<IDelta> dSites)
        {
            var physicalPathes = dSites.GetPhysicalPathChanges();

            foreach (var (newValue, _) in physicalPathes)
            {
                if (newValue is string path && !Directory.Exists(Environment.ExpandEnvironmentVariables(path)))
                {
                    var info = new DirectoryInfo(Environment.ExpandEnvironmentVariables(path));
                    var packageName = info.Parent?.Name;
                    var versionName = info.Name;

                    if (packageName != null || versionName != null)
                    {
                        var release = await _store.GetReleaseAsync(packageName, versionName);

                        if (release != null)
                        {
                            _logger.LogInformation("Download {package}@{version} ({url})", packageName, versionName, release.Url);

                            var stream = await _storage.DownloadAsync(release.Url);

                            info.CreateRecursive();

                            using var zip = new ZipArchive(stream, ZipArchiveMode.Read);
                            zip.ExtractToDirectory(info.FullName);
                        }
                    }

                    if (!Directory.Exists(info.FullName))
                        throw new InvalidOperationException($"Invalid package path. '{path}'");
                }
            }
        }

        public void DeleteOldDires(IEnumerable<IDelta> dSites)
        {
            var physicalPathes = dSites.GetPhysicalPathChanges();
            foreach (var (_, oldValue) in physicalPathes)
            {
                if (oldValue is string path && Directory.Exists(path))
                    Directory.Delete(path, true);
            }
        }

        public IEnumerable<string> RecycleApplicationPools(Partition partition)
        {
            return partition.ApplicationPools
                .Where(apppool => !_lastRecycleAt.ContainsKey(apppool.Name) || _lastRecycleAt[apppool.Name] < apppool.RecycleRequestAt)
                .Select(apppool => apppool.Name)
                .ToList();
        }

        public void SaveLastRecycleAt(Partition partition)
        {
            foreach (var apppool in partition.ApplicationPools)
            {
                _lastRecycleAt[apppool.Name] = apppool.RecycleRequestAt;
            }
        }

        public async Task Report()
        {
            var manager = new ServerManager();
            var applicationPools = _mapper.Map<ApplicationPool, POCO.ApplicationPool>(manager.ApplicationPools);
            var sites = _mapper.Map<Site, POCO.Site>(manager.Sites);

            await _store.SaveInstanceAsync(new VirtualMachine
            {
                ScaleSetName = _config.ScaleSetName,
                Name = Environment.MachineName,
                PartitionETag = _etag,
                Current = new Snapshot
                {
                    ScaleSetName = _config.ScaleSetName,
                    Name = Environment.MachineName,
                    Timestamp = DateTimeOffset.Now,
                    ApplicationPools = applicationPools.ToList(),
                    Sites = sites.ToList()
                }
            });
        }
    }
}
