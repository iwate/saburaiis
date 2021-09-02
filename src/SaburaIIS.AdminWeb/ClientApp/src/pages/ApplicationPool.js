import { DefaultButton, Stack, StackItem, TextField, IconButton, CommandBar } from "@fluentui/react";
import { useHistory, useParams } from "react-router"
import * as Breadcrumb from "../shared/Breadcrumb";
import { useApplicationPoolState, usePartitionState } from "../state";
import {
  managedPipelineModeOptions,
  startModeOptions,
  managedRuntimeVersionOptions,
  objectStateOptions,
  actionOptions,
  loadBalancerCapabilitiesOptions,
  identityTypeOptions,
  idleTimeoutActionOptions,
  logEventOnProcessModelOptions
} from "../constants";
import { 
  DropdownController, 
  TimeController, 
  TextController, 
  ToggleController,
  FlagController,
  DefaultValueController,
  IntegerController,
  PercentController,
  HexController
 } from "../parts/controllers";
import { useFieldArray, useForm } from "react-hook-form";
import { useEffect } from "react";
import { sameValues } from "../helper";

export const ApplicationPool = () => {
  const browserHistory = useHistory();
  const { partitionName, apppoolName } = useParams();
  const { removeApplicationPool } = usePartitionState(partitionName);
  const { origin, local, setLocal } = useApplicationPoolState(partitionName, apppoolName);
  const hasDiff = !sameValues(origin, local) && origin !== null && origin !== undefined;

  const { handleSubmit, control, formState: { isDirty }, reset } = useForm({
    mode: 'onChange',
    defaultValues: local,
  });

  const { fields: schedule, append: appendSchedule, remove: removeSchedule  } = useFieldArray({
    control,
    name: "recycling.periodicRestart.schedule",
    keyName: 'time'
  });


  useEffect(() => {
    if (local !== null) {
      reset({
        ...local,
        recycling: {
          ...local.recycling,
          periodicRestart: {
            ...local.recycling.periodicRestart,
          }
        }
      });
    }
  }, [partitionName, apppoolName, local, reset])

  Breadcrumb.useBreadcrumb([
    { text: partitionName, key: `/partitions/${partitionName}`, href: `/partitions/${partitionName}` },
    { text: 'Application Pools', key: `/partitions/${partitionName}/apppools/` },
    { text: apppoolName, key: `/partitions/${partitionName}/apppools/${apppoolName}` },
  ]);

  if (local === null) {
    return <div>Loading...</div>
  }


  const onSubmit = (data) => {
    console.log(data)
    setLocal(data);
    browserHistory.push(`/partitions/${partitionName}`);
  }

  const commands = [
    {
      key: 'remove',
      text: 'Remove',
      iconProps: { iconName: 'Clear' },
      onClick: () => {
        removeApplicationPool(apppoolName);
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
      <TextField label="State" value={objectStateOptions.find(item => item.key === local.state)?.text} readOnly />
      <DropdownController
        label="Managed Pipeline Mode"
        name="managedPipelineMode"
        options={managedPipelineModeOptions}
        control={control}
      />
      <DropdownController
        label="Managed Runtime Version"
        name="managedRuntimeVersion"
        options={managedRuntimeVersionOptions}
        control={control}
      />
      <DropdownController
        label="Start Mode"
        name="startMode"
        options={startModeOptions}
        control={control}
      />
      <IntegerController
        label="Queue Length"
        name="queueLength"
        control={control}
      />
      <ToggleController
        label="Auto Start"
        name="autoStart"
        onText="On"
        offText="Off"
        control={control}
      />
      <ToggleController
        label="Enable 32bit app on Win64"
        name="enable32BitAppOnWin64"
        onText="Enabled"
        offText="Disabled"
        control={control}
      />
      <StackItem>
        <h4>CPU</h4>
      </StackItem>
      <DropdownController
        label="Action"
        name="cpu.action"
        options={actionOptions}
        control={control}
      />
      <PercentController
        label="Limit"
        name="cpu.limit"
        control={control}
      />
      <TimeController
        label="Reset Interval"
        name="cpu.resetInterval"
        control={control}
      />
      <ToggleController
        label="Smp Affinitized"
        name="cpu.smpAffinitized"
        onText="Enabled"
        offText="Disabled"
        control={control}
      />
      <HexController
        label="Smp Processor Affinity Mask"
        name="cpu.smpProcessorAffinityMask"
        control={control}
      />
      <HexController
        label="Smp Processor Affinity Mask2"
        name="cpu.smpProcessorAffinityMask2"
        control={control}
      />
      <StackItem>
        <h4>Failure</h4>
      </StackItem>
      <DropdownController
        label="Load Balancer Capabilities"
        name="failure.loadBalancerCapabilities"
        options={loadBalancerCapabilitiesOptions}
        control={control}
      />
      <TextController
        label="Auto Shutdown Exe"
        name="failure.autoShutdownExe"
        control={control}
      />
      <TextController
        label="Auto Shutdown Params"
        name="failure.autoShutdownParams"
        control={control}
      />
      <ToggleController
        label="Orphan Worker Process"
        name="failure.orphanWorkerProcess"
        onText="Enabled"
        offText="Disabled"
        control={control}
      />
      <TextController
        label="Orphan Action Exe"
        name="failure.orphanActionExe"
        control={control}
      />
      <TextController
        label="Orphan Action Params"
        name="failure.orphanActionParams"
        control={control}
      />
      <ToggleController
        label="Rapid Fail Protection"
        name="failure.rapidFailProtection"
        onText="On"
        offText="Off"
        control={control}
      />
      <TimeController
        label="Rapid Fail Protection Interval"
        name="failure.rapidFailProtectionInterval"
        control={control}
      />
      <IntegerController
        label="Rapid Fail Protection Max Crashes"
        name="failure.rapidFailProtectionMaxCrashes"
        control={control}
      />
      <StackItem>
        <h4>Process Model</h4>
      </StackItem>
      <DropdownController
        label="Identity Type"
        name="processModel.identityType"
        options={identityTypeOptions}
        control={control}
      />
      <TextController
        label="User Name"
        name="processModel.userName"
        control={control}
      />
      <TextController
        label="Password"
        name="processModel.password"
        type="password"
        control={control}
      />
      <TimeController
        label="Idle Timeout"
        name="processModel.idleTimeout"
        control={control}
      />
      <DropdownController
        label="Idle Timeout Action"
        name="processModel.idleTimeoutAction"
        options={idleTimeoutActionOptions}
        control={control}
      />
      <ToggleController
        label="Load User Profile"
        name="processModel.loadUserProfile"
        onText="On"
        offText="Off"
        control={control}
      />
      <IntegerController
        label="Max Processes"
        name="processModel.maxProcesses"
        control={control}
      />
      <ToggleController
        label="Pinging Enabled"
        name="processModel.pingingEnabled"
        onText="Enabled"
        offText="Disabled"
        control={control}
      />
      <TimeController
        label="Ping Interval"
        name="processModel.pingInterval"
        control={control}
      />
      <TimeController
        label="Ping Response Time"
        name="processModel.pingResponseTime"
        control={control}
      />
      <TimeController
        label="Startup Time Limit"
        name="processModel.startupTimeLimit"
        control={control}
      />
      <TimeController
        label="Shutdown Time Limit"
        name="processModel.shutdownTimeLimit"
        control={control}
      />
      <DropdownController
        label="Log Event On Process Model"
        name="processModel.logEventOnProcessModel"
        options={logEventOnProcessModelOptions}
        control={control}
      />
      <StackItem>
        <h4>Recycling</h4>
      </StackItem>
      <ToggleController
        label="Disallow Overlapping Rotation"
        name="recycling.disallowOverlappingRotation"
        onText="Disallowed"
        offText="Allowed"
        control={control}
      />
      <ToggleController
        label="Disallow Rotation on Config Change"
        name="recycling.disallowRotationOnConfigChange"
        onText="Disallowed"
        offText="Allowed"
        control={control}
      />
      <FlagController
        labels={[
          "Log Event on Recycle: Time",
          "Log Event on Recycle: Requests",
          "Log Event on Recycle: Schedule",
          "Log Event on Recycle: Memory",
          "Log Event on Recycle: IsapiUnhealthy",
          "Log Event on Recycle: OnDemand",
          "Log Event on Recycle: ConfigChange",
          "Log Event on Recycle: PrivateMemory",
        ]}
        name="recycling.logEventOnRecycle"
        control={control}
      />
      <StackItem>
        <h4>Periodic Restart</h4>
      </StackItem>
      <IntegerController
        label="Memory"
        name="recycling.periodicRestart.memory"
        control={control}
      />
      <IntegerController
        label="Private Memory"
        name="recycling.periodicRestart.privateMemory"
        control={control}
      />
      <IntegerController
        label="Requests"
        name="recycling.periodicRestart.requests"
        control={control}
      />
      <TimeController
        label="Time"
        name="recycling.periodicRestart.time"
        control={control}
      />
      {schedule.map((item, index) =>
        <TimeController
          key={item.time}
          label={`Schedule ${index}`}
          name={`recycling.periodicRestart.schedule.${index}.time`}
          onRenderSuffix={() => <IconButton iconProps={{ iconName: 'clear' }} onClick={() => removeSchedule(index) }></IconButton>}
          control={control}
        />
      )}
      <DefaultValueController name="workerProcesses" control={control}/>
      <StackItem>
       <DefaultButton iconProps={{iconName:'Add'}} onClick={() => appendSchedule({ time: '00:00:00' })}>Add Schedule</DefaultButton>
      </StackItem>
    </Stack>
  </Stack>
}