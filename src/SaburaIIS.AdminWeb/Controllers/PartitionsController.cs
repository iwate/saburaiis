using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SaburaIIS.AdminWeb.ViewModels;
using SaburaIIS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SaburaIIS.AdminWeb.Controllers
{
    [ApiController]
    public class PartitionsController : ControllerBase
    {
        private readonly Config _config;
        private readonly Store _store;
        private readonly ILogger<PartitionsController> _logger;

        public PartitionsController(IOptions<Config> options, Store store, ILogger<PartitionsController> logger)
        {
            _config = options.Value;
            _store = store;
            _logger = logger;
        }

        [HttpGet("/api/partitions")]
        public async Task<IEnumerable<string>> GetPartitionNamesAsync()
        {
            return (await _store.GetPartitionNamesAsync()).OrderBy(name => name);
        }

        [HttpGet("/api/partitions/{name}")]
        public async Task<Partition> GetPartition(string name)
        {
            var (partition, etag) = await _store.GetPartitionAsync(name);
            Response.Headers.Add("ETag", etag);
            return partition;
        }

        [HttpPost("/api/partitions")]
        public async Task<IActionResult> PostPartition([FromBody]PartitionCreationViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _store.SavePartitionAsync(new Partition
            {
                id = model.Name,
                Name = model.Name,
                ApplicationPools =
                {
                    new POCO.ApplicationPool
                    {
                        Name = "DefaultAppPool",
                        AutoStart = true,
                        Enable32BitAppOnWin64 = false,
                        ManagedPipelineMode = POCO.ManagedPipelineMode.Integrated,
                        ManagedRuntimeVersion = "v4.0",
                        QueueLength = 1000,
                        StartMode = POCO.StartMode.OnDemand,
                        Cpu = new POCO.ApplicationPoolCpu
                        {
                            Action = POCO.ProcessorAction.NoAction,
                            Limit = 0,
                            ResetInterval = TimeSpan.FromMinutes(5),
                            SmpAffinitized = false,
                            SmpProcessorAffinityMask = 0xffffffff,
                            SmpProcessorAffinityMask2 = 0xffffffff,
                        },
                        Failure = new POCO.ApplicationPoolFailure
                        {
                            AutoShutdownExe = string.Empty,
                            AutoShutdownParams = string.Empty,
                            LoadBalancerCapabilities = POCO.LoadBalancerCapabilities.HttpLevel,
                            OrphanActionExe = string.Empty,
                            OrphanActionParams = string.Empty,
                            OrphanWorkerProcess = false,
                            RapidFailProtection = true,
                            RapidFailProtectionInterval = TimeSpan.FromMinutes(5),
                            RapidFailProtectionMaxCrashes = 5,
                        },
                        ProcessModel = new POCO.ApplicationPoolProcessModel
                        {
                            IdentityType = POCO.ProcessModelIdentityType.ApplicationPoolIdentity,
                            IdleTimeout = TimeSpan.FromMinutes(20),
                            IdleTimeoutAction = POCO.IdleTimeoutAction.Terminate,
                            LoadUserProfile = false,
                            MaxProcesses = 1,
                            PingingEnabled = true,
                            PingInterval = TimeSpan.FromSeconds(30),
                            PingResponseTime = TimeSpan.FromSeconds(90),
                            Password = string.Empty,
                            ShutdownTimeLimit = TimeSpan.FromSeconds(90),
                            StartupTimeLimit = TimeSpan.FromSeconds(90),
                            UserName = string.Empty,
                            LogEventOnProcessModel = POCO.ProcessModelLogEventOnProcessModel.IdleTimeout,
                        },
                        Recycling = new POCO.ApplicationPoolRecycling
                        {
                            DisallowOverlappingRotation = false,
                            DisallowRotationOnConfigChange = false,
                            LogEventOnRecycle = POCO.RecyclingLogEventOnRecycle.PrivateMemory
                                              | POCO.RecyclingLogEventOnRecycle.ConfigChange
                                              | POCO.RecyclingLogEventOnRecycle.OnDemand
                                              | POCO.RecyclingLogEventOnRecycle.IsapiUnhealthy
                                              | POCO.RecyclingLogEventOnRecycle.Memory
                                              | POCO.RecyclingLogEventOnRecycle.Schedule
                                              | POCO.RecyclingLogEventOnRecycle.Requests
                                              | POCO.RecyclingLogEventOnRecycle.Time,
                            PeriodicRestart = new POCO.ApplicationPoolPeriodicRestart
                            {
                                Memory = 0,
                                PrivateMemory = 0,
                                Requests = 0,
                                Schedule = new POCO.Schedule[]{ },
                                Time = TimeSpan.FromMilliseconds(104400_000),
                            }
                        }
                    }
                },
                Sites =
                {
                    new POCO.Site
                    {
                        Name = "Default Web Site",
                        ServerAutoStart = true,
                        Applications = new []
                        {
                            new POCO.Application
                            {
                                Path = "/",
                                ApplicationPoolName = "DefaultAppPool",
                                EnabledProtocols = "http",
                                VirtualDirectories = new[]
                                {
                                    new POCO.VirtualDirectory
                                    {
                                        Path = "/",
                                        PhysicalPath = _config.AppLocationDefault,
                                        LogonMethod = POCO.AuthenticationLogonMethod.ClearText,
                                        UserName = string.Empty,
                                        Password = string.Empty
                                    }
                                }
                            }
                        },
                        Bindings = new []
                        {
                            new POCO.Binding
                            {
                                BindingInformation = "*:80:",
                                Protocol = "http",
                                Host = string.Empty,
                                SslFlags = POCO.SslFlags.None,
                                CertificateStoreName = null,
                                CertificateHash = null,
                                IsIPPortHostBinding = true,
                                UseDsMapper = false
                            }
                        },
                        Limits = new POCO.SiteLimits
                        {
                            ConnectionTimeout = TimeSpan.FromSeconds(120),
                            MaxConnections = 4294967295,
                            MaxBandwidth = 4294967295,
                            MaxUrlSegments = 32,
                        },
                        LogFile = new POCO.SiteLogFile
                        {
                            Enabled = true,
                            Directory = _config.LogLocationDefault,
                            LogFormat = POCO.LogFormat.W3c,
                            LogTargetW3C = POCO.LogTargetW3C.File,
                            LogExtFileFlags = POCO.LogExtFileFlags.Date
                                            | POCO.LogExtFileFlags.Time
                                            | POCO.LogExtFileFlags.ClientIP
                                            | POCO.LogExtFileFlags.UserName
                                            | POCO.LogExtFileFlags.ServerIP
                                            | POCO.LogExtFileFlags.Method
                                            | POCO.LogExtFileFlags.UriStem
                                            | POCO.LogExtFileFlags.UriQuery
                                            | POCO.LogExtFileFlags.HttpStatus
                                            | POCO.LogExtFileFlags.Win32Status
                                            | POCO.LogExtFileFlags.TimeTaken
                                            | POCO.LogExtFileFlags.ServerPort
                                            | POCO.LogExtFileFlags.UserAgent
                                            | POCO.LogExtFileFlags.Referer
                                            | POCO.LogExtFileFlags.HttpSubStatus,
                            LocalTimeRollover = false,
                            Period = POCO.LoggingRolloverPeriod.Daily,
                            TruncateSize = 20971520,
                        },
                        TraceFailedRequestsLogging = new POCO.SiteTraceFailedRequestsLogging
                        {
                            Enabled = false,
                            Directory = @"%SystemDrive%\inetpub\logs\FailedReqLogFiles",
                            MaxLogFiles = 50,
                        },
                        HSTS = new POCO.SiteHSTS
                        {
                            Enabled = false,
                            MaxAge = 0,
                            IncludeSubDomains = false,
                            Preload = false,
                            RedirectHttpToHttps = false,
                        }
                    }
                },
            }, "*");

            return CreatedAtAction(nameof(GetPartition), null);
        }

        [HttpPut("/api/partitions/{name}")]
        public async Task<IActionResult> PutPartition([FromRoute]string name, [FromBody]Partition partition, [FromHeader(Name = "If-Match")] string etag)
        {
            if (name != partition.Name)
                return BadRequest();

            await _store.SavePartitionAsync(partition, etag);

            return NoContent();
        }

        [HttpDelete("/api/partitions/{name}")]
        public async Task<IActionResult> DeletePartition([FromRoute] string name, [FromHeader(Name = "If-Match")] string etag)
        {
            var (partition, _) = await _store.GetPartitionAsync(name);

            if (partition == null)
                return NotFound();

            if (partition.ScaleSets.Any())
                return BadRequest(new
                {
                    message = $"{name} has any scale sets settings."
                });

            await _store.RemovePartitionAsync(partition, etag);

            return NoContent();
        }

        [HttpGet("/api/partitions/{name}/instances")]
        public async Task<IActionResult> GetInstances([FromRoute] string name)
        {
            var (partition, _) = await _store.GetPartitionAsync(name);

            if (partition == null)
                return NotFound();

            var sets = await Task.WhenAll(partition.ScaleSets.Select(ss => _store.GetInstancesAsync(ss.Name)).ToArray());

            return Ok(sets.SelectMany(instances => instances));
        }

        [HttpGet("/api/partitions/{partitionName}/instances/{scaleSetName}/{instanceName}")]
        public async Task<IActionResult> GetInstance([FromRoute] string partitionName, [FromRoute] string scaleSetName, [FromRoute] string instanceName)
        {
            var (partition, _) = await _store.GetPartitionAsync(partitionName);

            if (partition == null)
                return NotFound();

            if (!partition.ScaleSets.Any(ss => ss.Name == scaleSetName))
                return NotFound();

            var instance = await _store.GetInstanceAsync(scaleSetName, instanceName);

            if (instance == null)
                return NotFound();

            return Ok(instance);
        }

        [HttpGet("/api/partitions/{partitionName}/instances/{scaleSetName}/{instanceName}/snapshots")]
        public async Task<IActionResult> GetSnapshots([FromRoute] string partitionName, [FromRoute] string scaleSetName, [FromRoute] string instanceName)
        {
            var (partition, _) = await _store.GetPartitionAsync(partitionName);

            if (partition == null)
                return NotFound();

            if (!partition.ScaleSets.Any(ss => ss.Name == scaleSetName))
                return NotFound();

            var snapshots = await _store.GetSnapshotsAsync(scaleSetName, instanceName);

            return Ok(snapshots);
        }

        [HttpGet("/api/partitions/{partitionName}/instances/{scaleSetName}/{instanceName}/snapshots/{timestamp}")]
        public async Task<IActionResult> GetSnapshots([FromRoute] string partitionName, [FromRoute] string scaleSetName, [FromRoute] string instanceName, [FromRoute] string timestamp)
        {
            var (partition, _) = await _store.GetPartitionAsync(partitionName);

            if (partition == null)
                return NotFound();

            if (!partition.ScaleSets.Any(ss => ss.Name == scaleSetName))
                return NotFound();

            var snapshot = await _store.GetSnapshotAsync(scaleSetName, instanceName, timestamp);

            if (snapshot == null)
                return NotFound();

            return Ok(snapshot);
        }
    }
}
