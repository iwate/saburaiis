using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using SaburaIIS.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SaburaIIS.Local
{
    public class FileStore : IStore
    {
        private readonly string _dir;

        private string PackagesDir => Path.Combine(_dir, "packages");
        private string PackageFile(string name) => Path.Combine(PackagesDir, name + ".json");
        private string PartitionsDir => Path.Combine(_dir, "partitions");
        private string PartitionFile(string name) => Path.Combine(PartitionsDir, name + ".json"); 
        private string InstancesDir => Path.Combine(_dir, "instances");
        private string InstanceFile(string name) => Path.Combine(InstancesDir, name + ".json");
        private string SnapshotsDir(string scaleSetName, string name) => Path.Combine(_dir, "snapshots", scaleSetName, name);
        private string SnapshotFile(string scaleSetName, string name, DateTimeOffset datetime) => Path.Combine(SnapshotsDir(scaleSetName, name), datetime.ToUnixTimeMilliseconds().ToString() + ".json");

        public FileStore(Config config)
        {
            _dir = config.FileStoreDirectoryPath;
            
            if (!Directory.Exists(_dir))
            {
                throw new DirectoryNotFoundException();
            }
        }
        public async Task AddSnapshoptAsync(Snapshot snapshot)
        {
            var dir = SnapshotsDir(snapshot.ScaleSetName, snapshot.Name);

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            var path = SnapshotFile(snapshot.ScaleSetName, snapshot.Name, snapshot.Timestamp);
            var json = JsonConvert.SerializeObject(snapshot);

            await File.WriteAllTextAsync(path, json);
        }

        public IChangeTracker CreateInstancesChangeTracker(string scaleSetName)
        {
            return new FileChangeTracker(InstanceFile(scaleSetName));
        }

        public IChangeTracker CreatePartitionChangeTracker(string partitionName)
        {
            return new FileChangeTracker(PartitionFile(partitionName));
        }

        public async Task<VirtualMachine> GetInstanceAsync(string scaleSetName, string name)
        {
            var path = InstanceFile(scaleSetName);

            if (!File.Exists(path))
            {
                return null;
            }

            var json = await File.ReadAllTextAsync(path);
            var scaleSet = JsonConvert.DeserializeObject<IEnumerable<VirtualMachine>>(json);

            return scaleSet.FirstOrDefault(item => item.Name == name);
        }

        public async Task<IEnumerable<VirtualMachine>> GetInstancesAsync(string scaleSetName, string etag = null)
        {
            var path = InstanceFile(scaleSetName);

            if (!File.Exists(path))
            {
                return Array.Empty<VirtualMachine>();
            }

            var json = await File.ReadAllTextAsync(path);
            var scaleSet = JsonConvert.DeserializeObject<IEnumerable<VirtualMachine>>(json);

            return scaleSet.Where(item => item.PartitionETag == etag);
        }

        public async Task<(Package, string)> GetPackageAsync(string name)
        {
            var path = PackageFile(name);

            if (!File.Exists(path))
            {
                return (null, null);
            }

            var json = await File.ReadAllTextAsync(path);
            var etag = GenerateETag(json);
            var package = JsonConvert.DeserializeObject<Package>(json);

            return (package, etag);
        }

        public virtual async Task RemovePackageAsync(Package package, string etag)
        {
            var path = PackageFile(package.Name);

            if (File.Exists(path))
            {
                var currentJson = await File.ReadAllTextAsync(path);
                var currentETag = GenerateETag(currentJson);

                if (currentETag != etag && etag != "*")
                {
                    throw new InvalidOperationException("Invalid ETag");
                }

                File.Delete(path);
            }
        }

        public Task<IEnumerable<string>> GetPackageNamesAsync()
        {
            var names = Directory.GetFiles(PackagesDir).Select(Path.GetFileNameWithoutExtension);
            return Task.FromResult(names);
        }

        public async Task<(Partition, string)> GetPartitionAsync(string name)
        {
            var path = PartitionFile(name);
            
            if (!File.Exists(path))
            {
                return (null, null);
            }

            var json = await File.ReadAllTextAsync(path);
            var etag = GenerateETag(json);
            var partition = JsonConvert.DeserializeObject<Partition>(json);

            return (partition, etag);
        }

        public Task<IEnumerable<string>> GetPartitionNamesAsync()
        {
            var names = Directory.GetFiles(PartitionsDir).Select(Path.GetFileNameWithoutExtension);
            return Task.FromResult(names);
        }

        public async Task<Release> GetReleaseAsync(string packageName, string version)
        {
            var path = PackageFile(packageName);

            if (!File.Exists(path))
            {
                return null;
            }

            var json = await File.ReadAllTextAsync(path);
            var package = JsonConvert.DeserializeObject<Package>(json);

            return package.Releases.FirstOrDefault(release => release.Version == version);
        }

        public async Task<IEnumerable<string>> GetReleaseVersionsAsync(string packageName, int limit = 30)
        {
            var path = PackageFile(packageName);

            if (!File.Exists(path))
            {
                return Array.Empty<string>();
            }

            var json = await File.ReadAllTextAsync(path);
            var package = JsonConvert.DeserializeObject<Package>(json);

            return package.Releases.OrderByDescending(release => release.ReleaseAt).Select(release => release.Version).ToList();
        }

        public async Task<Snapshot> GetSnapshotAsync(string scaleSetName, string name, string timestamp)
        {
            var path = SnapshotFile(scaleSetName, name, DateTimeOffset.Parse(timestamp));
            if (!File.Exists(path))
                return null;

            var json = await File.ReadAllTextAsync(path);

            var snapshot = JsonConvert.DeserializeObject<Snapshot>(json);

            return snapshot;
        }

        public Task<IEnumerable<DateTimeOffset>> GetSnapshotsAsync(string scaleSetName, string name)
        {
            var path = SnapshotsDir(scaleSetName, name);

            if (!Directory.Exists(path))
            {
                return Task.FromResult<IEnumerable<DateTimeOffset>>(Array.Empty<DateTimeOffset>());
            }

            var names = Directory.GetFiles(path).Select(Path.GetFileNameWithoutExtension);
            var timestamps = names.Select(unixtimestampStr => DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(unixtimestampStr)));

            return Task.FromResult<IEnumerable<DateTimeOffset>>(timestamps.OrderDescending());
        }

        public Task InitAsync()
        {
            if (!Directory.Exists(_dir))
            {
                throw new DirectoryNotFoundException(_dir);
            }

            if (!Directory.Exists(PackagesDir))
            {
                Directory.CreateDirectory(PackagesDir);
            }

            if (!Directory.Exists(PartitionsDir))
            {
                Directory.CreateDirectory(PartitionsDir);
            }

            if (!Directory.Exists(InstancesDir))
            {
                Directory.CreateDirectory(InstancesDir);
            }

            return Task.CompletedTask;
        }

        public async Task RemovePartitionAsync(Partition partition, string etag)
        {
            var path = PartitionFile(partition.Name);

            if (File.Exists(path))
            {
                var currentJson = await File.ReadAllTextAsync(path);
                var currentETag = GenerateETag(currentJson);

                if (currentETag != etag && etag != "*")
                {
                    throw new InvalidOperationException("Invalid ETag");
                }

                File.Delete(path);
            }
        }

        public async Task SaveInstanceAsync(VirtualMachine vm)
        {
            var path = InstanceFile(vm.ScaleSetName);

            List<VirtualMachine> scaleSet;
            if (File.Exists(path))
            {
                scaleSet = JsonConvert.DeserializeObject<List<VirtualMachine>>(await File.ReadAllTextAsync(path));

                var index = scaleSet.FindIndex(item => item.Name == vm.Name);

                if (index != -1)
                {
                    scaleSet.Insert(index, vm);
                }
                else
                {
                    scaleSet.Add(vm);
                }
            }
            else
            {
                scaleSet = new List<VirtualMachine>{ vm };
            }

            await File.WriteAllTextAsync(path, JsonConvert.SerializeObject(scaleSet));
        }

        public async Task SavePackageAsync(Package package, string etag)
        {
            var path = PackageFile(package.Name);

            if (File.Exists(path))
            {
                var currentJson = await File.ReadAllTextAsync(path);
                var currentETag = GenerateETag(currentJson);

                if (currentETag != etag && etag != "*")
                {
                    throw new InvalidOperationException("Invalid ETag");
                }
            }

            var json = JsonConvert.SerializeObject(package);

            await File.WriteAllTextAsync(path, json);
        }

        public async Task<string> SavePartitionAsync(Partition partition, string etag)
        {
            var path = PartitionFile(partition.Name);
            
            if (File.Exists(path))
            {
                var currentJson = await File.ReadAllTextAsync(path);
                var currentETag = GenerateETag(currentJson);

                if (currentETag != etag && etag != "*")
                {
                    throw new InvalidOperationException("Invalid ETag");
                }
            }

            var json = JsonConvert.SerializeObject(partition);
            
            await File.WriteAllTextAsync(path, json);
            
            return GenerateETag(json);
        }

        public async Task<string> SearchPartitionAsync(string scaleSetName)
        {
            var files = Directory.GetFiles(PartitionsDir);

            foreach (var file in files)
            {
                var partition = JsonConvert.DeserializeObject<Partition>(await File.ReadAllTextAsync(file));

                if (partition.ScaleSets.Any(s => s.Name == scaleSetName))
                {
                    return partition.Name;
                }
            }

            return null;
        }

        private string GenerateETag(string text)
        {
            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(text));
            return Convert.ToBase64String(hashBytes);
        }
    }
}
