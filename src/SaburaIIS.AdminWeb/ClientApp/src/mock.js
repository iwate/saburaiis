export const mock = () => [
    {
        "id": "stage01",
        "Name": "stage01",
        "ApplicationPools": [
            {
                "ManagedPipelineMode": 0,
                "ManagedRuntimeVersion": "v4.0",
                "StartMode": 0,
                "QueueLength": 1000,
                "AutoStart": true,
                "Enable32BitAppOnWin64": false,
                "Cpu": {
                    "Action": 0,
                    "Limit": 0,
                    "ResetInterval": "00:05:00",
                    "SmpProcessorAffinityMask": 4294967295,
                    "SmpProcessorAffinityMask2": 4294967295,
                    "SmpAffinitized": false
                },
                "Failure": {
                    "LoadBalancerCapabilities": 2,
                    "AutoShutdownExe": "",
                    "AutoShutdownParams": "",
                    "OrphanWorkerProcess": false,
                    "OrphanActionExe": "",
                    "OrphanActionParams": "",
                    "RapidFailProtection": true,
                    "RapidFailProtectionInterval": "00:05:00",
                    "RapidFailProtectionMaxCrashes": 5
                },
                "ProcessModel": {
                    "IdentityType": 4,
                    "UserName": "",
                    "Password": "",
                    "IdleTimeout": "00:20:00",
                    "IdleTimeoutAction": 0,
                    "LoadUserProfile": false,
                    "MaxProcesses": 1,
                    "PingingEnabled": true,
                    "PingInterval": "00:00:30",
                    "PingResponseTime": "00:01:30",
                    "StartupTimeLimit": "00:01:30",
                    "ShutdownTimeLimit": "00:01:30",
                    "LogEventOnProcessModel": 1
                },
                "Recycling": {
                    "DisallowOverlappingRotation": false,
                    "DisallowRotationOnConfigChange": false,
                    "LogEventOnRecycle": 255,
                    "PeriodicRestart": {
                        "Memory": 0,
                        "PrivateMemory": 0,
                        "Requests": 0,
                        "Time": "1.05:00:00"
                    }
                },
                "Name": "DefaultAppPool",
                "State": 1
            },
            {
                "ManagedPipelineMode": 1,
                "ManagedRuntimeVersion": "v4.0",
                "StartMode": 0,
                "QueueLength": 1000,
                "AutoStart": true,
                "Enable32BitAppOnWin64": false,
                "Cpu": {
                    "Action": 0,
                    "Limit": 0,
                    "ResetInterval": "00:05:00",
                    "SmpProcessorAffinityMask": 4294967295,
                    "SmpProcessorAffinityMask2": 4294967295,
                    "SmpAffinitized": false
                },
                "Failure": {
                    "LoadBalancerCapabilities": 2,
                    "AutoShutdownExe": "",
                    "AutoShutdownParams": "",
                    "OrphanWorkerProcess": false,
                    "OrphanActionExe": "",
                    "OrphanActionParams": "",
                    "RapidFailProtection": true,
                    "RapidFailProtectionInterval": "00:05:00",
                    "RapidFailProtectionMaxCrashes": 5
                },
                "ProcessModel": {
                    "IdentityType": 4,
                    "UserName": "",
                    "Password": "",
                    "IdleTimeout": "00:20:00",
                    "IdleTimeoutAction": 0,
                    "LoadUserProfile": false,
                    "MaxProcesses": 1,
                    "PingingEnabled": true,
                    "PingInterval": "00:00:30",
                    "PingResponseTime": "00:01:30",
                    "StartupTimeLimit": "00:01:30",
                    "ShutdownTimeLimit": "00:01:30",
                    "LogEventOnProcessModel": 1
                },
                "Recycling": {
                    "DisallowOverlappingRotation": false,
                    "DisallowRotationOnConfigChange": false,
                    "LogEventOnRecycle": 255,
                    "PeriodicRestart": {
                        "Memory": 0,
                        "PrivateMemory": 0,
                        "Requests": 0,
                        "Time": "1.05:00:00",
                        "Schedule": [
                            {
                                "Time": "00:00:00",
                                "id": "34994398-7ee4-4ff2-bd34-f63443f2bda9"
                            }
                        ]
                    }
                },
                "Name": ".NET v4.5 Classic",
                "State": 1
            },
            {
                "ManagedPipelineMode": 0,
                "ManagedRuntimeVersion": "v4.0",
                "StartMode": 0,
                "QueueLength": 1000,
                "AutoStart": true,
                "Enable32BitAppOnWin64": false,
                "Cpu": {
                    "Action": 0,
                    "Limit": 0,
                    "ResetInterval": "00:05:00",
                    "SmpProcessorAffinityMask": 4294967295,
                    "SmpProcessorAffinityMask2": 4294967295,
                    "SmpAffinitized": false
                },
                "Failure": {
                    "LoadBalancerCapabilities": 2,
                    "AutoShutdownExe": "",
                    "AutoShutdownParams": "",
                    "OrphanWorkerProcess": false,
                    "OrphanActionExe": "",
                    "OrphanActionParams": "",
                    "RapidFailProtection": true,
                    "RapidFailProtectionInterval": "00:05:00",
                    "RapidFailProtectionMaxCrashes": 5
                },
                "ProcessModel": {
                    "IdentityType": 4,
                    "UserName": "",
                    "Password": "",
                    "IdleTimeout": "00:20:00",
                    "IdleTimeoutAction": 0,
                    "LoadUserProfile": false,
                    "MaxProcesses": 1,
                    "PingingEnabled": true,
                    "PingInterval": "00:00:30",
                    "PingResponseTime": "00:01:30",
                    "StartupTimeLimit": "00:01:30",
                    "ShutdownTimeLimit": "00:01:30",
                    "LogEventOnProcessModel": 1
                },
                "Recycling": {
                    "DisallowOverlappingRotation": false,
                    "DisallowRotationOnConfigChange": false,
                    "LogEventOnRecycle": 255,
                    "PeriodicRestart": {
                        "Memory": 0,
                        "PrivateMemory": 0,
                        "Requests": 0,
                        "Time": "1.05:00:00"
                    }
                },
                "Name": ".NET v4.5",
                "State": 1
            }
        ],
        "Sites": [
            {
                "ApplicationDefaults": {
                    "ApplicationPoolName": "DefaultAppPool",
                    "EnabledProtocols": "http"
                },
                "Applications": [
                    {
                        "ApplicationPoolName": "DefaultAppPool",
                        "EnabledProtocols": "http",
                        "Path": "/",
                        "VirtualDirectories": [
                            {
                                "LogonMethod": 3,
                                "Password": "",
                                "Path": "/",
                                "PhysicalPath": "%SystemDrive%\\inetpub\\wwwroot",
                                "UserName": ""
                            }
                        ],
                        "VirtualDirectoryDefaults": {
                            "LogonMethod": 3,
                            "Password": "",
                            "UserName": ""
                        }
                    }
                ],
                "Bindings": [
                    {
                        "BindingInformation": "*:80:",
                        "CertificateHash": null,
                        "CertificateStoreName": null,
                        "Host": "",
                        "IsIPPortHostBinding": true,
                        "SslFlags": 0,
                        "UseDsMapper": false,
                        "Protocol": "http"
                    }
                ],
                "Id": 1,
                "Limits": {
                    "ConnectionTimeout": "00:02:00",
                    "MaxBandwidth": 4294967295,
                    "MaxConnections": 4294967295,
                    "MaxUrlSegments": 32
                },
                "LogFile": {
                    "Directory": "%SystemDrive%\\inetpub\\logs\\LogFiles",
                    "LocalTimeRollover": false,
                    "LogExtFileFlags": 2478031,
                    "Enabled": true,
                    "CustomLogFields": [],
                    "CustomLogPluginClsid": "00000000-0000-0000-0000-000000000000",
                    "Period": 1,
                    "LogFormat": 2,
                    "LogTargetW3C": 1,
                    "TruncateSize": 20971520
                },
                "Name": "Default Web Site",
                "ServerAutoStart": true,
                "State": 1,
                "TraceFailedRequestsLogging": {
                    "Directory": "%SystemDrive%\\inetpub\\logs\\FailedReqLogFiles",
                    "Enabled": false,
                    "MaxLogFiles": 50
                },
                "HSTS": {
                    "Enabled": false,
                    "MaxAge": 0,
                    "IncludeSubDomains": false,
                    "Preload": false,
                    "RedirectHttpToHttps": false
                },
                "VirtualDirectoryDefaults": {
                    "LogonMethod": 3,
                    "Password": "",
                    "UserName": ""
                }
            }
        ],
        "ScaleSets": [
            {
                "Name": "sample"
            }
        ],
        "Packages": [],
        "$etag": "2021-08-05T08:36:40.703Z"
    },
    {
        "id": "production01",
        "Name": "production01",
        "ApplicationPools": [
            {
                "ManagedPipelineMode": 0,
                "ManagedRuntimeVersion": "v4.0",
                "StartMode": 0,
                "QueueLength": 1000,
                "AutoStart": true,
                "Enable32BitAppOnWin64": false,
                "Cpu": {
                    "Action": 0,
                    "Limit": 0,
                    "ResetInterval": "00:05:00",
                    "SmpProcessorAffinityMask": 4294967295,
                    "SmpProcessorAffinityMask2": 4294967295,
                    "SmpAffinitized": false
                },
                "Failure": {
                    "LoadBalancerCapabilities": 2,
                    "AutoShutdownExe": "",
                    "AutoShutdownParams": "",
                    "OrphanWorkerProcess": false,
                    "OrphanActionExe": "",
                    "OrphanActionParams": "",
                    "RapidFailProtection": true,
                    "RapidFailProtectionInterval": "00:05:00",
                    "RapidFailProtectionMaxCrashes": 5
                },
                "ProcessModel": {
                    "IdentityType": 4,
                    "UserName": "",
                    "Password": "",
                    "IdleTimeout": "00:20:00",
                    "IdleTimeoutAction": 0,
                    "LoadUserProfile": false,
                    "MaxProcesses": 1,
                    "PingingEnabled": true,
                    "PingInterval": "00:00:30",
                    "PingResponseTime": "00:01:30",
                    "StartupTimeLimit": "00:01:30",
                    "ShutdownTimeLimit": "00:01:30",
                    "LogEventOnProcessModel": 1
                },
                "Recycling": {
                    "DisallowOverlappingRotation": false,
                    "DisallowRotationOnConfigChange": false,
                    "LogEventOnRecycle": 255,
                    "PeriodicRestart": {
                        "Memory": 0,
                        "PrivateMemory": 0,
                        "Requests": 0,
                        "Time": "1.05:00:00"
                    }
                },
                "Name": "DefaultAppPool",
                "State": 1
            },
            {
                "ManagedPipelineMode": 1,
                "ManagedRuntimeVersion": "v4.0",
                "StartMode": 0,
                "QueueLength": 1000,
                "AutoStart": true,
                "Enable32BitAppOnWin64": false,
                "Cpu": {
                    "Action": 0,
                    "Limit": 0,
                    "ResetInterval": "00:05:00",
                    "SmpProcessorAffinityMask": 4294967295,
                    "SmpProcessorAffinityMask2": 4294967295,
                    "SmpAffinitized": false
                },
                "Failure": {
                    "LoadBalancerCapabilities": 2,
                    "AutoShutdownExe": "",
                    "AutoShutdownParams": "",
                    "OrphanWorkerProcess": false,
                    "OrphanActionExe": "",
                    "OrphanActionParams": "",
                    "RapidFailProtection": true,
                    "RapidFailProtectionInterval": "00:05:00",
                    "RapidFailProtectionMaxCrashes": 5
                },
                "ProcessModel": {
                    "IdentityType": 4,
                    "UserName": "",
                    "Password": "",
                    "IdleTimeout": "00:20:00",
                    "IdleTimeoutAction": 0,
                    "LoadUserProfile": false,
                    "MaxProcesses": 1,
                    "PingingEnabled": true,
                    "PingInterval": "00:00:30",
                    "PingResponseTime": "00:01:30",
                    "StartupTimeLimit": "00:01:30",
                    "ShutdownTimeLimit": "00:01:30",
                    "LogEventOnProcessModel": 1
                },
                "Recycling": {
                    "DisallowOverlappingRotation": false,
                    "DisallowRotationOnConfigChange": false,
                    "LogEventOnRecycle": 255,
                    "PeriodicRestart": {
                        "Memory": 0,
                        "PrivateMemory": 0,
                        "Requests": 0,
                        "Time": "1.05:00:00",
                        "Schedule": [
                            {
                                "Time": "00:00:00",
                                "id": "34994398-7ee4-4ff2-bd34-f63443f2bda9"
                            }
                        ]
                    }
                },
                "Name": ".NET v4.5 Classic",
                "State": 1
            },
            {
                "ManagedPipelineMode": 0,
                "ManagedRuntimeVersion": "v4.0",
                "StartMode": 0,
                "QueueLength": 1000,
                "AutoStart": true,
                "Enable32BitAppOnWin64": false,
                "Cpu": {
                    "Action": 0,
                    "Limit": 0,
                    "ResetInterval": "00:05:00",
                    "SmpProcessorAffinityMask": 4294967295,
                    "SmpProcessorAffinityMask2": 4294967295,
                    "SmpAffinitized": false
                },
                "Failure": {
                    "LoadBalancerCapabilities": 2,
                    "AutoShutdownExe": "",
                    "AutoShutdownParams": "",
                    "OrphanWorkerProcess": false,
                    "OrphanActionExe": "",
                    "OrphanActionParams": "",
                    "RapidFailProtection": true,
                    "RapidFailProtectionInterval": "00:05:00",
                    "RapidFailProtectionMaxCrashes": 5
                },
                "ProcessModel": {
                    "IdentityType": 4,
                    "UserName": "",
                    "Password": "",
                    "IdleTimeout": "00:20:00",
                    "IdleTimeoutAction": 0,
                    "LoadUserProfile": false,
                    "MaxProcesses": 1,
                    "PingingEnabled": true,
                    "PingInterval": "00:00:30",
                    "PingResponseTime": "00:01:30",
                    "StartupTimeLimit": "00:01:30",
                    "ShutdownTimeLimit": "00:01:30",
                    "LogEventOnProcessModel": 1
                },
                "Recycling": {
                    "DisallowOverlappingRotation": false,
                    "DisallowRotationOnConfigChange": false,
                    "LogEventOnRecycle": 255,
                    "PeriodicRestart": {
                        "Memory": 0,
                        "PrivateMemory": 0,
                        "Requests": 0,
                        "Time": "1.05:00:00"
                    }
                },
                "Name": ".NET v4.5",
                "State": 1
            }
        ],
        "Sites": [
            {
                "ApplicationDefaults": {
                    "ApplicationPoolName": "DefaultAppPool",
                    "EnabledProtocols": "http"
                },
                "Applications": [
                    {
                        "ApplicationPoolName": "DefaultAppPool",
                        "EnabledProtocols": "http",
                        "Path": "/",
                        "VirtualDirectories": [
                            {
                                "LogonMethod": 3,
                                "Password": "",
                                "Path": "/",
                                "PhysicalPath": "%SystemDrive%\\inetpub\\wwwroot",
                                "UserName": ""
                            }
                        ],
                        "VirtualDirectoryDefaults": {
                            "LogonMethod": 3,
                            "Password": "",
                            "UserName": ""
                        }
                    }
                ],
                "Bindings": [
                    {
                        "BindingInformation": "*:80:",
                        "CertificateHash": null,
                        "CertificateStoreName": null,
                        "Host": "",
                        "IsIPPortHostBinding": true,
                        "SslFlags": 0,
                        "UseDsMapper": false,
                        "Protocol": "http"
                    }
                ],
                "Id": 1,
                "Limits": {
                    "ConnectionTimeout": "00:02:00",
                    "MaxBandwidth": 4294967295,
                    "MaxConnections": 4294967295,
                    "MaxUrlSegments": 32
                },
                "LogFile": {
                    "Directory": "%SystemDrive%\\inetpub\\logs\\LogFiles",
                    "LocalTimeRollover": false,
                    "LogExtFileFlags": 2478031,
                    "Enabled": true,
                    "CustomLogFields": [],
                    "CustomLogPluginClsid": "00000000-0000-0000-0000-000000000000",
                    "Period": 1,
                    "LogFormat": 2,
                    "LogTargetW3C": 1,
                    "TruncateSize": 20971520
                },
                "Name": "Default Web Site",
                "ServerAutoStart": true,
                "State": 1,
                "TraceFailedRequestsLogging": {
                    "Directory": "%SystemDrive%\\inetpub\\logs\\FailedReqLogFiles",
                    "Enabled": false,
                    "MaxLogFiles": 50
                },
                "HSTS": {
                    "Enabled": false,
                    "MaxAge": 0,
                    "IncludeSubDomains": false,
                    "Preload": false,
                    "RedirectHttpToHttps": false
                },
                "VirtualDirectoryDefaults": {
                    "LogonMethod": 3,
                    "Password": "",
                    "UserName": ""
                }
            }
        ],
        "ScaleSets": [
            {
                "Name": "sample",
                "Instances": []
            }
        ],
        "Packages": [],
        "$etag": "2021-08-05T08:36:40.703Z"
    },
    {
        "id": "production02",
        "Name": "production02",
        "ApplicationPools": [
            {
                "ManagedPipelineMode": 0,
                "ManagedRuntimeVersion": "v4.0",
                "StartMode": 0,
                "QueueLength": 1000,
                "AutoStart": true,
                "Enable32BitAppOnWin64": false,
                "Cpu": {
                    "Action": 0,
                    "Limit": 0,
                    "ResetInterval": "00:05:00",
                    "SmpProcessorAffinityMask": 4294967295,
                    "SmpProcessorAffinityMask2": 4294967295,
                    "SmpAffinitized": false
                },
                "Failure": {
                    "LoadBalancerCapabilities": 2,
                    "AutoShutdownExe": "",
                    "AutoShutdownParams": "",
                    "OrphanWorkerProcess": false,
                    "OrphanActionExe": "",
                    "OrphanActionParams": "",
                    "RapidFailProtection": true,
                    "RapidFailProtectionInterval": "00:05:00",
                    "RapidFailProtectionMaxCrashes": 5
                },
                "ProcessModel": {
                    "IdentityType": 4,
                    "UserName": "",
                    "Password": "",
                    "IdleTimeout": "00:20:00",
                    "IdleTimeoutAction": 0,
                    "LoadUserProfile": false,
                    "MaxProcesses": 1,
                    "PingingEnabled": true,
                    "PingInterval": "00:00:30",
                    "PingResponseTime": "00:01:30",
                    "StartupTimeLimit": "00:01:30",
                    "ShutdownTimeLimit": "00:01:30",
                    "LogEventOnProcessModel": 1
                },
                "Recycling": {
                    "DisallowOverlappingRotation": false,
                    "DisallowRotationOnConfigChange": false,
                    "LogEventOnRecycle": 255,
                    "PeriodicRestart": {
                        "Memory": 0,
                        "PrivateMemory": 0,
                        "Requests": 0,
                        "Time": "1.05:00:00"
                    }
                },
                "Name": "DefaultAppPool",
                "State": 1
            },
            {
                "ManagedPipelineMode": 1,
                "ManagedRuntimeVersion": "v4.0",
                "StartMode": 0,
                "QueueLength": 1000,
                "AutoStart": true,
                "Enable32BitAppOnWin64": false,
                "Cpu": {
                    "Action": 0,
                    "Limit": 0,
                    "ResetInterval": "00:05:00",
                    "SmpProcessorAffinityMask": 4294967295,
                    "SmpProcessorAffinityMask2": 4294967295,
                    "SmpAffinitized": false
                },
                "Failure": {
                    "LoadBalancerCapabilities": 2,
                    "AutoShutdownExe": "",
                    "AutoShutdownParams": "",
                    "OrphanWorkerProcess": false,
                    "OrphanActionExe": "",
                    "OrphanActionParams": "",
                    "RapidFailProtection": true,
                    "RapidFailProtectionInterval": "00:05:00",
                    "RapidFailProtectionMaxCrashes": 5
                },
                "ProcessModel": {
                    "IdentityType": 4,
                    "UserName": "",
                    "Password": "",
                    "IdleTimeout": "00:20:00",
                    "IdleTimeoutAction": 0,
                    "LoadUserProfile": false,
                    "MaxProcesses": 1,
                    "PingingEnabled": true,
                    "PingInterval": "00:00:30",
                    "PingResponseTime": "00:01:30",
                    "StartupTimeLimit": "00:01:30",
                    "ShutdownTimeLimit": "00:01:30",
                    "LogEventOnProcessModel": 1
                },
                "Recycling": {
                    "DisallowOverlappingRotation": false,
                    "DisallowRotationOnConfigChange": false,
                    "LogEventOnRecycle": 255,
                    "PeriodicRestart": {
                        "Memory": 0,
                        "PrivateMemory": 0,
                        "Requests": 0,
                        "Time": "1.05:00:00",
                        "Schedule": [
                            {
                                "Time": "00:00:00",
                                "id": "34994398-7ee4-4ff2-bd34-f63443f2bda9"
                            }
                        ]
                    }
                },
                "Name": ".NET v4.5 Classic",
                "State": 1
            },
            {
                "ManagedPipelineMode": 0,
                "ManagedRuntimeVersion": "v4.0",
                "StartMode": 0,
                "QueueLength": 1000,
                "AutoStart": true,
                "Enable32BitAppOnWin64": false,
                "Cpu": {
                    "Action": 0,
                    "Limit": 0,
                    "ResetInterval": "00:05:00",
                    "SmpProcessorAffinityMask": 4294967295,
                    "SmpProcessorAffinityMask2": 4294967295,
                    "SmpAffinitized": false
                },
                "Failure": {
                    "LoadBalancerCapabilities": 2,
                    "AutoShutdownExe": "",
                    "AutoShutdownParams": "",
                    "OrphanWorkerProcess": false,
                    "OrphanActionExe": "",
                    "OrphanActionParams": "",
                    "RapidFailProtection": true,
                    "RapidFailProtectionInterval": "00:05:00",
                    "RapidFailProtectionMaxCrashes": 5
                },
                "ProcessModel": {
                    "IdentityType": 4,
                    "UserName": "",
                    "Password": "",
                    "IdleTimeout": "00:20:00",
                    "IdleTimeoutAction": 0,
                    "LoadUserProfile": false,
                    "MaxProcesses": 1,
                    "PingingEnabled": true,
                    "PingInterval": "00:00:30",
                    "PingResponseTime": "00:01:30",
                    "StartupTimeLimit": "00:01:30",
                    "ShutdownTimeLimit": "00:01:30",
                    "LogEventOnProcessModel": 1
                },
                "Recycling": {
                    "DisallowOverlappingRotation": false,
                    "DisallowRotationOnConfigChange": false,
                    "LogEventOnRecycle": 255,
                    "PeriodicRestart": {
                        "Memory": 0,
                        "PrivateMemory": 0,
                        "Requests": 0,
                        "Time": "1.05:00:00"
                    }
                },
                "Name": ".NET v4.5",
                "State": 1
            }
        ],
        "Sites": [
            {
                "ApplicationDefaults": {
                    "ApplicationPoolName": "DefaultAppPool",
                    "EnabledProtocols": "http"
                },
                "Applications": [
                    {
                        "ApplicationPoolName": "DefaultAppPool",
                        "EnabledProtocols": "http",
                        "Path": "/",
                        "VirtualDirectories": [
                            {
                                "LogonMethod": 3,
                                "Password": "",
                                "Path": "/",
                                "PhysicalPath": "%SystemDrive%\\inetpub\\wwwroot",
                                "UserName": ""
                            }
                        ],
                        "VirtualDirectoryDefaults": {
                            "LogonMethod": 3,
                            "Password": "",
                            "UserName": ""
                        }
                    }
                ],
                "Bindings": [
                    {
                        "BindingInformation": "*:80:",
                        "CertificateHash": null,
                        "CertificateStoreName": null,
                        "Host": "",
                        "IsIPPortHostBinding": true,
                        "SslFlags": 0,
                        "UseDsMapper": false,
                        "Protocol": "http"
                    }
                ],
                "Id": 1,
                "Limits": {
                    "ConnectionTimeout": "00:02:00",
                    "MaxBandwidth": 4294967295,
                    "MaxConnections": 4294967295,
                    "MaxUrlSegments": 32
                },
                "LogFile": {
                    "Directory": "%SystemDrive%\\inetpub\\logs\\LogFiles",
                    "LocalTimeRollover": false,
                    "LogExtFileFlags": 2478031,
                    "Enabled": true,
                    "CustomLogFields": [],
                    "CustomLogPluginClsid": "00000000-0000-0000-0000-000000000000",
                    "Period": 1,
                    "LogFormat": 2,
                    "LogTargetW3C": 1,
                    "TruncateSize": 20971520
                },
                "Name": "Default Web Site",
                "ServerAutoStart": true,
                "State": 1,
                "TraceFailedRequestsLogging": {
                    "Directory": "%SystemDrive%\\inetpub\\logs\\FailedReqLogFiles",
                    "Enabled": false,
                    "MaxLogFiles": 50
                },
                "HSTS": {
                    "Enabled": false,
                    "MaxAge": 0,
                    "IncludeSubDomains": false,
                    "Preload": false,
                    "RedirectHttpToHttps": false
                },
                "VirtualDirectoryDefaults": {
                    "LogonMethod": 3,
                    "Password": "",
                    "UserName": ""
                }
            }
        ],
        "ScaleSets": [
            {
                "Name": "sample",
                "Instances": []
            }
        ],
        "Packages": [],
        "$etag": "2021-08-05T08:36:40.703Z"
    }
]