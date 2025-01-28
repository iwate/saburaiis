export const managedPipelineModeOptions = [
  { key: 0, text: 'Integrated' },
  { key: 1, text: 'Classic' },
]

export const startModeOptions = [
  { key: 0, text: 'OnDemand' },
  { key: 1, text: 'AlwaysRunning' },
]

export const managedRuntimeVersionOptions = [
  { key: 'v4.0', text: 'v4.0' },
  { key: 'v2.0', text: 'v2.0' },
  { key: 'v1.1', text: 'v1.1' },
]

export const objectStateOptions = [
  { key: 0, text: 'Starting' },
  { key: 1, text: 'Started' },
  { key: 2, text: 'Stopping' },
  { key: 3, text: 'Stopped' },
  { key: 4, text: 'Unknown' },
]

export const actionOptions = [
  { key: 0, text: 'NoAction' },
  { key: 1, text: 'KillW3wp' },
  { key: 2, text: 'Throttle' },
  { key: 3, text: 'ThrottleUnderLoad' },
]

export const loadBalancerCapabilitiesOptions = [
  { key: 1, text: 'TcpLevel' },
  { key: 2, text: 'HttpLevel' },
]

export const identityTypeOptions = [
  { key: 0, text: 'LocalSystem' },
  { key: 1, text: 'LocalService' },
  { key: 2, text: 'NetworkService' },
  { key: 3, text: 'SpecificUser' },
  { key: 4, text: 'ApplicationPoolIdentity' },
]

export const idleTimeoutActionOptions = [
  { key: 0, text: 'Terminate' },
  { key: 1, text: 'Suspend' },
]

export const logEventOnProcessModelOptions = [
  { key: 0, text: 'None' },
  { key: 1, text: 'IdleTimeout' },
]

export const logFilePeriodOptions = [
  { key: 0, text: 'MaxSize' },
  { key: 1, text: 'Daily' },
  { key: 2, text: 'Weekly' },
  { key: 3, text: 'Monthly' },
  { key: 4, text: 'Hourly' },
]

export const logFormatOptions = [
  { key: 0, text: 'IIS' },
  { key: 1, text: 'NCSA' },
  { key: 2, text: 'W3C' },
  { key: 3, text: 'Custom' },
]

export const logTargetW3COptions = [
  { key: 1, text: 'File' },
  { key: 2, text: 'ETW' },
]

export const sslFlagsOptions = [
  { key: 0, text: 'None' },
  { key: 1, text: 'SNI' },
  { key: 2, text: 'CentralCertStore' },
  { key: 3, text: 'DisableHTTP2' },
  { key: 4, text: 'DisableOCSPStp' },
  { key: 5, text: 'DisableQUIC' },
  { key: 6, text: 'DisableTLS13' },
  { key: 7, text: 'DisableLegacyTLS' },
  { key: 8, text: 'NegotiateClientCert' },
]

export const logonMethodOptions = [
  { key: 0, text: 'Interactive' },
  { key: 1, text: 'Batch' },
  { key: 2, text: 'Network' },
  { key: 3, text: 'ClearText' },
]

export const protocolOptions = [
  { key: 'http', text: 'http' },
  { key: 'https', text: 'https' },
]

export const certStoreOptions = [
  { key: '', text: '' },
  { key: 'My', text: 'My' },
  { key: 'Root', text: 'Root' },
]

export const DEFAULT_APPLICATION_POOL = {
  "autoStart": true,
  "cpu": {
    "action": 0,
    "limit": 0,
    "resetInterval": "00:05:00",
    "smpAffinitized": false,
    "smpProcessorAffinityMask": 4294967295,
    "smpProcessorAffinityMask2": 4294967295
  },
  "enable32BitAppOnWin64": false,
  "enableEmulationOnWinArm64": false,
  "failure": {
    "autoShutdownExe": "",
    "autoShutdownParams": "",
    "loadBalancerCapabilities": 2,
    "orphanActionExe": "",
    "orphanActionParams": "",
    "orphanWorkerProcess": false,
    "rapidFailProtection": true,
    "rapidFailProtectionInterval": "00:05:00",
    "rapidFailProtectionMaxCrashes": 5
  },
  "managedPipelineMode": 0,
  "managedRuntimeVersion": "v4.0",
  "name": "DefaultAppPool",
  "processModel": {
    "identityType": 4,
    "idleTimeout": "00:20:00",
    "idleTimeoutAction": 0,
    "loadUserProfile": false,
    "logEventOnProcessModel": 1,
    "maxProcesses": 1,
    "password": "",
    "pingInterval": "00:00:30",
    "pingResponseTime": "00:01:30",
    "pingingEnabled": true,
    "shutdownTimeLimit": "00:01:30",
    "startupTimeLimit": "00:01:30",
    "userName": ""
  },
  "queueLength": 1000,
  "recycling": {
    "disallowOverlappingRotation": false,
    "disallowRotationOnConfigChange": false,
    "logEventOnRecycle": 255,
    "periodicRestart": {
      "memory": 0,
      "privateMemory": 0,
      "requests": 0,
      "schedule": [],
      "time": "1.05:00:00"
    }
  },
  "startMode": 0,
  "state": 1,
  "workerProcesses": []
};
export const DEFAULT_SITE = {
  "applications": [
    {
      "applicationPoolName": "DefaultAppPool",
      "enabledProtocols": "http",
      "path": "/",
      "virtualDirectories": [
        {
          "logonMethod": 3,
          "password": "",
          "path": "/",
          "physicalPath": "%SystemDrive%\\inetpub\\wwwroot",
          "userName": ""
        }
      ]
    }
  ],
  "bindings": [
    {
      "bindingInformation": "*:80:",
      "certificateHash": null,
      "certificateStoreName": null,
      "host": "",
      "isIPPortHostBinding": true,
      "protocol": "http",
      "sslFlags": 0,
      "useDsMapper": false
    }
  ],
  "hsts": {
    "enabled": false,
    "includeSubDomains": false,
    "maxAge": 0,
    "preload": false,
    "redirectHttpToHttps": false
  },
  "limits": {
    "connectionTimeout": "00:02:00",
    "maxBandwidth": 4294967295,
    "maxConnections": 4294967295,
    "maxUrlSegments": 32
  },
  "logFile": {
    "customLogFields": [],
    "customLogPluginClsid": "00000000-0000-0000-0000-000000000000",
    "directory": "%SystemDrive%\\inetpub\\logs\\LogFiles",
    "enabled": true,
    "localTimeRollover": false,
    "logExtFileFlags": 2478031,
    "logFormat": 2,
    "logTargetW3C": 1,
    "period": 1,
    "truncateSize": 20971520
  },
  "name": "Default Web Site",
  "serverAutoStart": true,
  "state": 1,
  "traceFailedRequestsLogging": {
    "directory": "%SystemDrive%\\inetpub\\logs\\FailedReqLogFiles",
    "enabled": false,
    "maxLogFiles": 50
  }
}