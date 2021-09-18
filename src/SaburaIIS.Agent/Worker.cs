using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Web.Administration;
using SaburaIIS.Agent.Mappers;
using SaburaIIS.Agent.Transformers;
using SaburaIIS.Models;

namespace SaburaIIS.Agent
{
    public class Worker : BackgroundService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<Worker> _logger;
        private readonly Store _store;
        private readonly Storage _storage;
        private readonly KeyVault _keyVault;
        private readonly Mapper _mapper;
        private readonly string _vmss;
        private ServerManager _manager;
        private ChangeTracker<Partition>? _tracker;
        private FileSystemWatcher _watcher;

        public Worker(IOptions<Config> options, IHttpClientFactory httpClientFactory, ILogger<Worker> logger)
        {
            var config = options.Value;

            _httpClient = httpClientFactory.CreateClient();
            _logger = logger;
            _store = new Store(config);
            _storage = new Storage(config);
            _keyVault = new KeyVault(config);
            _mapper = new Mapper();
            _manager = new ServerManager();
            _watcher = new FileSystemWatcher();
            _vmss = config.ScaleSetName!;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Partition? partition = null;

            while(true)
            {
                try
                {
                    await _store.InitAsync();
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while initializing store.");
                    await Task.Delay(TimeSpan.FromSeconds(60));
                }
            }

            try
            {
                partition = await GetCurrent(partition, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while initializing partition.");
                return;
            }

            try
            {
                await StartReporting();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while starting reporting");
                return;
            }

            var first = true;
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if (!first && !await _tracker!.HasChangeAsync())
                        continue;

                    partition = await GetCurrent(partition, stoppingToken);

                    _logger.LogInformation("Check {partition} changes", partition.Name);

                    var applicationPools = _mapper.Map<ApplicationPool, POCO.ApplicationPool>(_manager.ApplicationPools).ToList();
                    var sites = _mapper.Map<Site, POCO.Site>(_manager.Sites).ToList();

                    var dAppPools = Delta.CreateCollection(applicationPools, partition.ApplicationPools).ToList();
                    var dSites = Delta.CreateCollection(sites, partition.Sites).ToList();

                    if (dAppPools.Count == 0 && dSites.Count == 0)
                        continue;

                    _logger.LogInformation("Detect {partition} changes", partition.Name);

                    await DeployCertificates(partition.Sites);

                    await DeployPackages(dSites, async () =>
                    {
                        _logger.LogInformation("Transform application pools");

                        foreach (var delta in dAppPools)
                            Transformer.Transform(_manager.ApplicationPools, delta);

                        _logger.LogInformation("Transform sites");

                        foreach (var delta in dSites)
                            Transformer.Transform(_manager.Sites, delta);

                        _logger.LogInformation("Save snapshot");

                        await _store.AddSnapshoptAsync(new Snapshot
                        {
                            ScaleSetName = _vmss,
                            Name = Environment.MachineName,
                            Timestamp = DateTimeOffset.Now,
                            ApplicationPools = applicationPools.ToList(),
                            Sites = sites.ToList()
                        });

                        _logger.LogInformation("Commit changes");

                        _manager.CommitChanges();

                        RecycleAppPools(dSites);
                    });

                    first = false;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while work");
                    ResetManager();
                }
                finally
                {
                    await Task.Delay(1000, stoppingToken);
                }
            }
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
        async Task<Partition> GetCurrent(Partition? partition, CancellationToken stoppingToken)
        {
            if (partition == null || !partition.ScaleSets.Any(ss => ss.Name == _vmss))
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    var name = await _store.SearchPartitionAsync(_vmss);
                    if (name != null)
                    {
                        var (current, _) = await _store.GetPartitionAsync(name);
                        ResetTracker(name);
                        return current;
                    }
                    await Task.Delay(15000, stoppingToken);
                }

                return partition!;
            }
            else
            {
                var (current, _) = await _store.GetPartitionAsync(partition.Name);
                return current;
            }
        }

        void RecycleAppPools(IEnumerable<IDelta> delta)
        {
            var apppools = new List<string>();
            var sites = _manager.Sites.ToList();
            foreach (var dsite in delta)
            {
                var site = sites.First(s => s.Name == (string?)dsite.Key);
                foreach (var dapp in dsite.NestCollectionProperties["Applications"])
                {
                    if (dapp.Method == DeltaMethod.Update && dapp.HasDiff)
                    {
                        var app = site.Applications.First(a => a.Path == (string?)dapp.Key);
                        apppools.Add(app.ApplicationPoolName);
                    }
                }
            }

            foreach (var apppoolName in apppools)
            {
                _logger.LogInformation("Recycle '{apppoolName}'", apppoolName);
                _manager.ApplicationPools.First(apppool => apppool.Name == apppoolName).Recycle();
            }
        }

        async Task ReportInstance()
        {
            var manager = new ServerManager();
            var applicationPools = _mapper.Map<ApplicationPool, POCO.ApplicationPool>(manager.ApplicationPools);
            var sites = _mapper.Map<Site, POCO.Site>(manager.Sites);

            await _store.SaveInstanceAsync(new VirtualMachine
            {
                ScaleSetName = _vmss,
                Name = Environment.MachineName,
                Current = new Snapshot
                {
                    ScaleSetName = _vmss,
                    Name = Environment.MachineName,
                    Timestamp = DateTimeOffset.Now,
                    ApplicationPools = applicationPools.ToList(),
                    Sites = sites.ToList()
                }
            });
        }

        async Task StartReporting()
        {
            await ReportInstance();
            _watcher.Path = Environment.ExpandEnvironmentVariables(@"%SystemRoot%\System32\inetsrv\Config");
            _watcher.Filter = "*.config";
            _watcher.NotifyFilter = NotifyFilters.LastWrite;
            _watcher.IncludeSubdirectories = false;
            _watcher.Changed += async (object sender, FileSystemEventArgs e) => {
                try
                {
                    await ReportInstance();
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while reporting instance changes");
                }
            };
            _watcher.EnableRaisingEvents = true;
        }

        async Task DeployCertificates(IEnumerable<POCO.Site> sites)
        {
            var certs = sites.SelectMany(site =>
                                site.Bindings
                                    .Where(binding => binding.CertificateHash != null)
                                    .Select(binding => (binding.CertificateHash, binding.CertificateStoreName)))
                            .Distinct()
                            .GroupBy(cert => cert.CertificateStoreName);

            if (!certs.Any())
                return;

            var registry = await _keyVault.GetCertificatesAsync();
            
            foreach(var group in certs)
            {
                using var store = new X509Store(group.Key, StoreLocation.LocalMachine);
                store.Open(OpenFlags.ReadWrite);
                var notStoredCerts = group.Where(cert => store.Certificates.Find(X509FindType.FindByThumbprint, Convert.ToHexString(cert.CertificateHash), false).Count == 0);
                foreach (var cert in notStoredCerts)
                {
                    var info = registry.First(o => o.Thumbprint.SequenceEqual(cert.CertificateHash));
                    var raw = await _keyVault.GetCertificateAsync(info.Name, info.Version);
                    if (raw != null)
                    {
                        store.Add(new X509Certificate2(raw, (string?)null, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet));
                    }
                }
                store.Close();
            }
        }

        async Task DeployPackages(IEnumerable<IDelta> dSites, Func<Task> action)
        {
            var physicalPathes = 
                dSites.SelectMany(dsite =>
                    dsite.NestCollectionProperties["Applications"].SelectMany(dapp => 
                        dapp.NestCollectionProperties["VirtualDirectories"]
                            .Where(dvdir => dvdir.ValueProperties.ContainsKey("PhysicalPath"))
                            .Select(dvdir => dvdir.ValueProperties["PhysicalPath"])));

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
                            Stream stream;
                            try
                            {
                                stream = await _storage.DownloadAsync(release.Url);
                            }
                            catch(Exception ex)
                            {
                                _logger.LogError(ex, "An error occurred while downloading package");
                                throw new InvalidOperationException($"Cannot download the package release. '{path}'");
                            }

                            MakeDirs(info);

                            using var zip = new ZipArchive(stream, ZipArchiveMode.Read);
                            zip.ExtractToDirectory(info.FullName);
                        }
                    }

                    if (!Directory.Exists(info.FullName))
                        throw new InvalidOperationException($"Invalid package path. '{path}'");
                }
            }
            try
            {
                await action();
            }
            catch
            {
                foreach (var (newValue, _) in physicalPathes)
                {
                    if (newValue is string path && Directory.Exists(path))
                        Directory.Delete(path, true);
                }
                throw;
            }
            foreach (var (_, oldValue) in physicalPathes)
            {
                if (oldValue is string path && Directory.Exists(path))
                    Directory.Delete(path, true);
            }
        }

        public void MakeDirs(DirectoryInfo? info)
        {
            var dirs = new Stack<string>();
            while(info != null && !info.Exists)
            {
                dirs.Push(info.FullName);
                info = info.Parent;
            }
            while(dirs.TryPop(out var path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }
}
