using SaburaIIS.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SaburaIIS
{
    public interface IStore
    {
        Task InitAsync();
        /// <summary>
        /// Persist partition data
        /// </summary>
        /// <param name="partition">A partition object</param>
        /// <param name="etag">Current ETag string of the partition</param>
        /// <returns>New ETag string</returns>
        Task<string> SavePartitionAsync(Partition partition, string etag);

        /// <summary>
        /// Delete partition data
        /// </summary>
        /// <param name="partition">A partition object</param>
        /// <param name="etag">Current ETag string of the partition</param>
        /// <returns></returns>
        Task RemovePartitionAsync(Partition partition, string etag);

        /// <summary>
        /// List partiion name
        /// </summary>
        /// <returns>A list of all partition name</returns>
        Task<IEnumerable<string>> GetPartitionNamesAsync();

        /// <summary>
        /// Search partition list by VM set name
        /// </summary>
        /// <param name="scaleSetName">VM set name</param>
        /// <returns>A partition name</returns>
        Task<string> SearchPartitionAsync(string scaleSetName);

        /// <summary>
        /// Get a partition object with ETag
        /// </summary>
        /// <param name="name">partition name</param>
        /// <returns>Partition object and ETag string</returns>
        Task<(Partition, string)> GetPartitionAsync(string name);

        /// <summary>
        /// Persist a package data
        /// </summary>
        /// <param name="package">A package object</param>
        /// <param name="etag">Current ETag string of the package</param>
        /// <returns></returns>
        Task SavePackageAsync(Package package, string etag);

        /// <summary>
        /// List package name
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<string>> GetPackageNamesAsync();

        /// <summary>
        /// Get a package object with ETag
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Package object and ETag string</returns>
        Task<(Package, string)> GetPackageAsync(string name);

        /// <summary>
        /// List release versions of a package sorted by newer is at the top.
        /// </summary>
        /// <param name="packageName">Package name</param>
        /// <returns>A list of release version names</returns>
        Task<IEnumerable<string>> GetReleaseVersionsAsync(string packageName, int limit = 30);

        /// <summary>
        /// Get a release object
        /// </summary>
        /// <param name="packageName">Package name</param>
        /// <param name="version">Release version name</param>
        /// <returns>A Release object</returns>
        Task<Release> GetReleaseAsync(string packageName, string version);

        /// <summary>
        /// Persist a Virtual Machine data
        /// </summary>
        /// <param name="vm">VirtualMachine object</param>
        /// <returns></returns>
        Task SaveInstanceAsync(VirtualMachine vm);

        /// <summary>
        /// List a virtual machine data
        /// </summary>
        /// <param name="scaleSetName">Grouping name for VMs</param>
        /// <param name="etag">Current ETag string or null</param>
        /// <returns>List of VirtualMachine objects</returns>
        Task<IEnumerable<VirtualMachine>> GetInstancesAsync(string scaleSetName, string etag = null);

        /// <summary>
        /// Get a virtual machine data
        /// </summary>
        /// <param name="scaleSetName">Grouping name for VMs</param>
        /// <param name="name"></param>
        /// <returns></returns>
        Task<VirtualMachine> GetInstanceAsync(string scaleSetName, string name);

        /// <summary>
        /// Persist a snapshot of a virtual machine
        /// </summary>
        /// <param name="snapshot"></param>
        /// <returns></returns>
        Task AddSnapshoptAsync(Snapshot snapshot);

        /// <summary>
        /// List snapshot timestamps of a virtual machine
        /// </summary>
        /// <param name="scaleSetName">Grouping name for VMs</param>
        /// <param name="name">Snapshot name</param>
        /// <returns>List of timestamps of the snapshot</returns>
        Task<IEnumerable<DateTimeOffset>> GetSnapshotsAsync(string scaleSetName, string name);

        /// <summary>
        /// Get a snapshot data of a virtual machine
        /// </summary>
        /// <param name="scaleSetName">Grouping name for VMs</param>
        /// <param name="name">Snapshot name</param>
        /// <param name="timestamp">Timestamp</param>
        /// <returns>Snapshot object</returns>
        Task<Snapshot> GetSnapshotAsync(string scaleSetName, string name, string timestamp);

        /// <summary>
        /// Create a change tracker for a partition data
        /// </summary>
        /// <param name="partitionName"></param>
        /// <returns>ChangeTracker object</returns>
        IChangeTracker CreatePartitionChangeTracker(string partitionName);

        /// <summary>
        /// Create a change tracker for a virtual machine data
        /// </summary>
        /// <param name="scaleSetName">Grouping name for VMs</param>
        /// <returns>ChangeTracker object</returns>
        IChangeTracker CreateInstancesChangeTracker(string scaleSetName);

    }
}
