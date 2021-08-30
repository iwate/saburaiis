import { CommandBar, DefaultButton, IconButton, Stack, StackItem, TextField } from "@fluentui/react"
import { useEffect } from "react";
import { useFieldArray, useForm } from "react-hook-form";
import { useHistory, useParams } from "react-router"
import { useBreadcrumb } from "../shared/Breadcrumb";
import { useApplicationPoolListValue, usePartitionState, useSiteState } from "../state";
import { 
  DropdownController,
  ToggleController,
  TextController,
  IntegerController,
  FlagController,
  NullableTextController,
  TimeController,
  DefaultValueController,
  PhysicalPathController
 } from "../parts/controllers";
import { logFilePeriodOptions, logFormatOptions, logonMethodOptions, logTargetW3COptions, objectStateOptions } from "../constants";
import { sameValues } from "../helper";



export const Site = () => {
  const browserHistory = useHistory();
  const { partitionName, siteName } = useParams();
  const { removeSite } = usePartitionState(partitionName); 
  const { origin, local, setLocal } = useSiteState(partitionName, siteName);
  const { local: applicationPools } = useApplicationPoolListValue(partitionName);
  const hasDiff = !sameValues(origin, local) && origin !== null && origin !== undefined;

  const { handleSubmit, control, formState: { isDirty }, reset } = useForm({
    mode: 'onChange',
    defaultValues: local,
  })

  const { fields: applications, append: appendApplication, remove: removeApplication } = useFieldArray({
    control,
    name: 'applications',
    keyName: 'path'
  });

  const { fields: bindings, append: appendBinding, remove: removeBinding } = useFieldArray({
    control,
    name: 'bindings',
    keyName: 'bindingInformation'
  });

  useEffect(() => {
    if (local !== null) {
      reset({
        ...local,
      });
    }
  }, [partitionName, siteName, local])

  useBreadcrumb([
    { text: partitionName, key: `/partitions/${partitionName}`, href: `/partitions/${partitionName}` },
    { text: 'Sites', key: `/partitions/${partitionName}/sites/` },
    { text: siteName, key: `/partitions/${partitionName}/sites/${siteName}` },
  ]);

  if (local === null) {
    return <div>Loading...</div>
  }

  const onSubmit = (data) => {
    console.log(data)
    setLocal(data);
    browserHistory.push(`/partitions/${partitionName}`);
  }

  const applicationPoolOptions = applicationPools.map(name => ({
    key: name,
    text: name,
  }))

  const commands = [
    {
      key: 'remove',
      text: 'Remove',
      iconProps: { iconName: 'Clear' },
      onClick: () => {
        removeSite(siteName);
        browserHistory.push(`/partitions/${partitionName}`);
      },
    },
    {
      key: 'stage',
      text: 'Stage',
      iconProps: { iconName: 'Add' },
      disabled: !isDirty,
      onClick: () => handleSubmit(onSubmit)(),
    },
    {
      key: 'discard',
      text: 'Discard',
      iconProps: { iconName: 'Undo' },
      disabled: !hasDiff,
      onClick: () => setLocal(origin),
    }
  ]

  return <Stack style={{ width: '100%' }}>
    <CommandBar items={commands} style={{ borderBottom: '1px solid #eee', paddingBottom: 4 }} />
    <Stack tokens={{ childrenGap: 8, padding: 16 }} style={{ width: '100%', overflowY: 'auto' }}>
      <TextField label="Name" value={local.name} readOnly />
      <TextField label="State" value={objectStateOptions.find(item => item.key === local.state).text} readOnly />
      <ToggleController
        label="Server Auto Start"
        name="serverAutoStart"
        onText="On"
        offText="Off"
        control={control}
      />
      <StackItem><h4>Applications</h4></StackItem>
      {applications.map((item, index) =>
        <Stack tokens={{childrenGap: 8}} key={index}>
          <TextController
            label={`#${index} Path`}
            name={`applications.${index}.path`}
            onRenderSuffix={() => <IconButton iconProps={{ iconName: 'clear' }} onClick={() => removeApplication(index)}></IconButton>}
            control={control}
          />
          <DropdownController
            label={`#${index} Application Pool `}
            name={`applications.${index}.applicationPoolName`}
            options={applicationPoolOptions}
            control={control}
          />
          <TextController
            label={`#${index} Enabled Protocols`}
            name={`applications.${index}.enabledProtocols`}
            control={control}
          />
          <StackItem><h5>#{index} Virtual Directories</h5></StackItem>
          <VirtualDirectoryController
            control={control}
            labelPrefix={``}
            namePrefix={`applications.${index}.virtualDirectories`}
          />
        </Stack>
      )}
      <StackItem>
        <DefaultButton
          iconProps={{ iconName: 'Add' }}
          onClick={() => appendApplication({
            path: '',
            applicationPoolName: applicationPools[0] || null,
            enabledProtocols: 'http',
            virtualDirectories: [
              {
                logonMethod: 3,
                path: '/',
                physicalPath: '%SystemDrive%\\inetpub\\wwwroot',
                userName: '',
                password: '',
              }
            ]
          })}
        >Add Application</DefaultButton>
      </StackItem>
      <StackItem><h4>Bindings</h4></StackItem>
      {bindings.map((item, index) =>
        <Stack tokens={{childrenGap: 8}} key={index}>
          <TextController
            label={`#${index} Binding Information`}
            name={`bindings.${index}.bindingInformation`}
            onRenderSuffix={() => <IconButton iconProps={{ iconName: 'clear' }} onClick={() => removeBinding(index)}></IconButton>}
            control={control}
          />
          <TextController
            label={`#${index} Protocol`}
            name={`bindings.${index}.protocol`}
            control={control}
          />
          <TextController
            label={`#${index} Host`}
            name={`bindings.${index}.host`}
            control={control}
          />
          <FlagController
            labels={[
              `#${index} SSL: None`,
              `#${index} SSL: SNI`,
              `#${index} SSL: Central Cert Store`,
              `#${index} SSL: Disable HTTP2`,
              `#${index} SSL: Disable OCSPStp`,
              `#${index} SSL: Disable QUIC`,
              `#${index} SSL: Disable TLS1.3`,
              `#${index} SSL: Disable Legacy TLS`,
            ]}
            name={`bindings.${index}.sslFlags`}
            control={control}
          />
          <NullableTextController
            label={`#${index} CertificateStoreName`}
            name={`bindings.${index}.certificateStoreName`}
            control={control}
          />
          <NullableTextController
            label={`#${index} CertificateHash`}
            name={`bindings.${index}.certificateHash`}
            control={control}
          />
          <ToggleController
            label={`#${index} IsIPPortHostBinding`}
            name={`bindings.${index}.isIPPortHostBinding`}
            onText="Yes"
            offText="No"
            control={control}
          />
          <ToggleController
            label={`#${index} UseDsMapper`}
            name={`bindings.${index}.useDsMapper`}
            onText="Yes"
            offText="No"
            control={control}
          />
        </Stack>
      )}
      <StackItem>
        <DefaultButton
          iconProps={{ iconName: 'Add' }}
          onClick={() => appendBinding({
            bindingInformation: '*:80:',
            protocol: '',
            host: '',
            sslFlags: 0,
            certificateStoreName: null,
            certificateHash: null,
            isIPPortHostBinding: false,
            useDsMapper: false,
          })}
        >Add Bindings</DefaultButton>
      </StackItem>
      <StackItem><h4>Limits</h4></StackItem>
      <TimeController
        label="Connection Timeout"
        name="limits.connectionTimeout"
        control={control}
      />
      <IntegerController
        label="Max Bandwidth"
        name="limits.maxBandwidth"
        control={control}
      />
      <IntegerController
        label="Max Connections"
        name="limits.maxConnections"
        control={control}
      />
      <IntegerController
        label="Max Url Segments"
        name="limits.maxUrlSegments"
        control={control}
      />
      <StackItem><h4>Log File</h4></StackItem>
      <ToggleController
        label="Enabled"
        name="logFile.enabled"
        onText="Enabled"
        offText="Disabled"
        control={control}
      />
      <TextController
        label="Directory"
        name="logFile.directory"
        control={control}
      />
      <DropdownController
        label="LogFormat"
        name="logFile.logFormat"
        options={logFormatOptions}
        control={control}
      />
      <DropdownController
        label="Log Target W3C"
        name="logFile.logTargetW3C"
        options={logTargetW3COptions}
        control={control}
      />
      <FlagController
        labels={[
          "Log Ext File Flags: Date",
          "Log Ext File Flags: Time",
          "Log Ext File Flags: Client IP",
          "Log Ext File Flags: User Name",
          "Log Ext File Flags: Site Name",
          "Log Ext File Flags: Computer Name",
          "Log Ext File Flags: Server IP",
          "Log Ext File Flags: Method",
          "Log Ext File Flags: Uri Stem",
          "Log Ext File Flags: Uri Query",
          "Log Ext File Flags: Http Status",
          "Log Ext File Flags: Win32 Status",
          "Log Ext File Flags: Bytes Sent",
          "Log Ext File Flags: Bytes Recv",
          "Log Ext File Flags: Time Taken",
          "Log Ext File Flags: Server Port",
          "Log Ext File Flags: User Agent",
          "Log Ext File Flags: Cookie",
          "Log Ext File Flags: Referer",
          "Log Ext File Flags: Protocol Version",
          "Log Ext File Flags: Host",
          "Log Ext File Flags: Http Sub Status"
        ]}
        name="logFile.logExtFileFlags"
        control={control}
      />

      <ToggleController
        label="Local Time Rollover"
        name="logFile.localTimeRollover"
        onText="On"
        offText="Off"
        control={control}
      />
      <DropdownController
        label="Period"
        name="logFile.period"
        options={logFilePeriodOptions}
        control={control}
      />
      <IntegerController
        label="Truncate Size"
        name="logFile.truncateSize"
        control={control}
      />
      <DefaultValueController name="logFile.customLogFields" control={control}/>
      <StackItem><h4>Trace Failed Requests Logging</h4></StackItem>
      <ToggleController
        label="Enabled"
        name="traceFailedRequestsLogging.enabled"
        onText="Enabled"
        offText="Disabled"
        control={control}
      />
      <TextController
        label="Directory"
        name="traceFailedRequestsLogging.directory"
        control={control}
      />
      <IntegerController
        label="Max Log Files"
        name="traceFailedRequestsLogging.maxLogFiles"
        control={control}
      />
      <StackItem><h4>HSTS</h4></StackItem>
      <ToggleController
        label="Enabled"
        name="hsts.enabled"
        onText="Enabled"
        offText="Disabled"
        control={control}
      />
      <IntegerController
        label="Max Age"
        name="hsts.maxAge"
        control={control}
      />
      <ToggleController
        label="Include Sub Domains"
        name="hsts.includeSubDomains"
        onText="Enabled"
        offText="Disabled"
        control={control}
      />
      <ToggleController
        label="Preload"
        name="hsts.preload"
        onText="Enabled"
        offText="Disabled"
        control={control}
      />
      <ToggleController
        label="Redirect Http to Https"
        name="hsts.redirectHttpToHttps"
        onText="Enabled"
        offText="Disabled"
        control={control}
      />
    </Stack>
  </Stack>
}

const VirtualDirectoryController = ({control, namePrefix, labelPrefix}) => {
  const { fields: virtualDirectories, append: appendVirtualDirectory, remove: removeVirtualDirectory } = useFieldArray({
    control,
    name: namePrefix,
    keyName: 'path'
  })
  return <>
    {virtualDirectories.map((item, index) =>
      <Stack tokens={{childrenGap: 8}} key={index}>
        <TextController
          label={`${labelPrefix} #${index} Path`}
          name={`${namePrefix}.${index}.path`}
          onRenderSuffix={() => <IconButton iconProps={{ iconName: 'clear' }} onClick={() => removeVirtualDirectory(index)}></IconButton>}
          control={control}
        />
        <PhysicalPathController
          label={`${labelPrefix} #${index} Physical Path`}
          name={`${namePrefix}.${index}.physicalPath`}
          control={control}
        />
        <DropdownController
          label={`${labelPrefix} #${index} LogonMethod`}
          name={`${namePrefix}.${index}.logonMethod`}
          options={logonMethodOptions}
          control={control}
        />
        <TextController
          label={`${labelPrefix} #${index} User Name`}
          name={`${namePrefix}.${index}.userName`}
          control={control}
        />
        <TextController
          label={`${labelPrefix} #${index} Password`}
          name={`${namePrefix}.${index}.password`}
          control={control}
        />
      </Stack>
    )}
    <StackItem>
      <DefaultButton 
        iconProps={{iconName: 'Add'}}
        onClick={() => appendVirtualDirectory({
          path: '/',
          physicalPath: '%SystemDrive%\\inetpub\\wwwroot',
          logonMethod: 3,
          userName: '',
          password: ''
        })}
      >Add Virtual Directory</DefaultButton>
    </StackItem>
  </>
}