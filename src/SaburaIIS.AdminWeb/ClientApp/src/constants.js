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
]

export const logonMethodOptions = [
  { key: 0, text: 'Interactive' },
  { key: 1, text: 'Batch' },
  { key: 2, text: 'Network' },
  { key: 3, text: 'ClearText' },
]