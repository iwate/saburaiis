export const getPartitionNames = async () => {
  const response = await fetch(`/api/partitions`);
  if (!response.ok)
    throw "fetch error";
  return await response.json(); 
}

export const getPartition = async (partitionName) => {
  const response = await fetch(`/api/partitions/${partitionName}`);
  if (!response.ok)
    throw "fetch error";
  const data = await response.json();
  data['@etag'] = response.headers.get('etag');
  return data;
}

export const replacePartition = async (partition) => {
  const response = await fetch(`/api/partitions/${partition.name}`, {
    method: 'put',
    headers: { 'Content-Type': 'application/json', 'If-Match': partition['@etag'] },
    body: JSON.stringify(partition)
  });
  if (!response.ok)
    throw "fetch error";
  return response.headers.get('etag');
}

export const addPartition = async (name) => {
  const response = await fetch(`/api/partitions/`, {
    method: 'post',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({name})
  });
  if (!response.ok)
    throw (await getErrorMessage(response));
}

export const removePartition = async (partition) => {
  const response = await fetch(`/api/partitions/${partition.name}`, {
    method: 'delete',
    headers: { 'If-Match': partition['@etag'] }
  });
  if (!response.ok)
    throw (await getErrorMessage(response));
}

export const getInstances = async (partitionName) => {
  const response = await fetch(`/api/partitions/${partitionName}/instances`);
  if (!response.ok)
    throw "fetch error";
  return await response.json();
}

export const getInstance = async (partitionName, scaleSetName, instanceName) => {
  const response = await fetch(`/api/partitions/${partitionName}/instances/${scaleSetName}/${instanceName}`);
  if (!response.ok)
    throw "fetch error";
  return await response.json();
}

export const getSnapshots = async (partitionName, scaleSetName, instanceName) => {
  const response = await fetch(`/api/partitions/${partitionName}/instances/${scaleSetName}/${instanceName}/snapshots`);
  if (!response.ok)
    throw "fetch error";
  return await response.json();
}

export const getSnapshot = async (partitionName, scaleSetName, instanceName, timestamp) => {
  const response = await fetch(`/api/partitions/${partitionName}/instances/${scaleSetName}/${instanceName}/snapshots/${encodeURIComponent(timestamp)}`);
  if (!response.ok)
    throw "fetch error";
  return await response.json();
}

export const getPackageSummary = async () => {
  const response = await fetch(`/api/packages/`);
  if (!response.ok)
    throw "fetch error";
  return await response.json();
}

export const getReleaseVersions = async (packageName) => {
  const response = await fetch(`/api/packages/${packageName}/releases`);
  if (!response.ok)
    throw "fetch error";
  return await response.json();
}

export const addPackage = async (name) => {
  const response = await fetch(`/api/packages/`, {
    method: 'post',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ name })
  });
  if (!response.ok)
    throw (await getErrorMessage(response));
}

export const addRelease = async (packageName, version, url) => {
  const response = await fetch(`/api/packages/${packageName}/releases`, {
    method: 'post',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ version, url })
  });
  if (!response.ok)
    throw (await getErrorMessage(response));
}

export const getRelease = async (packageName, versionName) => {
  const response = await fetch(`/api/packages/${packageName}/releases/${versionName}`);
  if (!response.ok)
    throw "fetch error";
  return await response.json();
}

export const getCertificates = async () => {
  const response = await fetch(`/api/certificates`);
  if (!response.ok)
    throw "fetch error";
  return await response.json();
}

const getErrorMessage = async (response) => {
  if (response.headers.has('Content-Type')
   && response.headers.get('Content-Type').match(/application\/[^\s]*json/)) {
    const data = await response.json();
    if (data.errors) 
      return Object.values(data.errors).join('\n');
    if (data.message)
      return data.message;
    if (data.title)
      return data.title
    return JSON.stringify(data);
  }
  else {
    return await response.text();
  }
}
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