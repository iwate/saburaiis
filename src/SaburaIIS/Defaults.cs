using SaburaIIS.Models;
using System;

namespace SaburaIIS
{
    public class Defaults
    {
        public static Partition CreatePartition(string name, Config config) => new Partition
        {
            id = name,
            Name = name,
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
                                        PhysicalPath = config.AppLocationDefault,
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
                            Directory = config.LogLocationDefault,
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
        };
    }
}
