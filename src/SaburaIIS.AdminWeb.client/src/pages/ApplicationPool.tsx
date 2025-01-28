import { DefaultButton, Stack, StackItem, IconButton, CommandBar } from "@fluentui/react";
import { useParams } from "react-router"
import {
  managedPipelineModeOptions,
  startModeOptions,
  managedRuntimeVersionOptions,
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
} from "../assemblies/Controllers";
import { useFieldArray, useForm } from "react-hook-form";
import { useContext, useEffect, useMemo } from "react";
import { sameValues } from "../helper";
import { useBreadcrumbs } from "../assemblies/Breadcrumbs";
import { WorkingContext } from "../components/WorkingContext";
import usePartitionState from "../hooks/usePartitionState";
import { useNavigate } from "react-router";

type RouteParams = {
  partitionName: string
  apppoolName: string
}

export default function ApplicationPool() {
  const navigate = useNavigate();
  const { partitionName, apppoolName } = useParams<RouteParams>();
  const [partition] = usePartitionState(partitionName!)
  const { value: context, actions: { setPartition } } = useContext(WorkingContext);
  const origin = useMemo(() => partition?.applicationPools.find(x => x.name === apppoolName), [partition, apppoolName]);
  const local = useMemo(() => context.partitions[partitionName!]?.applicationPools.find(x => x.name === apppoolName), [context, partitionName, apppoolName]);
  const hasDiff = useMemo(() => !sameValues(origin, local) && origin !== null && origin !== undefined, [origin, local]);

  const { handleSubmit, control, formState: { isDirty }, reset } = useForm({
    mode: 'onChange',
    defaultValues: local!,
  });

  const { fields: schedule, append: appendSchedule, remove: removeSchedule } = useFieldArray({
    control,
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    name: "recycling.periodicRestart.schedule" as any as never,
    keyName: 'time'
  });
  
  useEffect(() => {
    if (local) {
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
  }, [local]) // eslint-disable-line react-hooks/exhaustive-deps

  useBreadcrumbs([
    { text: partitionName!, key: `/partitions/${partitionName}`, href: `/partitions/${partitionName}` },
    { text: 'Application Pools', key: `/partitions/${partitionName}/apppools/` },
    { text: apppoolName!, key: `/partitions/${partitionName}/apppools/${apppoolName}` },
  ]);

  const onSubmit: Parameters<typeof handleSubmit>[0] = (data) => {
    const partition = context.partitions[partitionName!];
    const applicationPools = [...partition.applicationPools];
    const index = applicationPools.findIndex(x => x.name === apppoolName);
    applicationPools[index] = { ...data };
    const local = {
      ...partition,
      applicationPools
    };
    const hasDiff = !sameValues(origin, local) && origin !== null && origin !== undefined;
    setPartition(local, hasDiff || context.diffs[partitionName!] || false);
    navigate(`/partitions/${partitionName}`);
  }

  const commands = [
    {
      key: 'remove',
      text: 'Remove',
      iconProps: { iconName: 'Clear' },
      onClick: () => {
        const partition = context.partitions[partitionName!];
        const applicationPools = [...partition.applicationPools.filter(x => x.name === apppoolName)];
        const local = {
          ...partition,
          applicationPools
        };
        const hasDiff = !sameValues(origin, local) && origin !== null && origin !== undefined;
        setPartition(local, hasDiff || context.diffs[partitionName!] || false);
        navigate(`/partitions/${partitionName}`);
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
      onClick: () => {
        setPartition({ ...context.partitions[partitionName!] }, hasDiff);
      },
    },
    {
      key: 'recycle',
      text: 'Recycle',
      iconProps: { iconName: 'Refresh' },
      disabled: isDirty,
      onClick: () => {
        const partition = context.partitions[partitionName!];
        const applicationPools = [...partition.applicationPools];
        const index = applicationPools.findIndex(x => x.name === apppoolName);
        applicationPools[index] = {
          ...applicationPools[index],
          recycleRequestAt: new Date().toISOString()
        }
        const local = {
          ...partition,
          applicationPools
        };
        setPartition(local, true);
        navigate(`/partitions/${partitionName}`);
      },
    }
  ]
  return <Stack style={{ width: '100%' }}>
    <CommandBar items={commands} style={{ borderBottom: '1px solid #eee', paddingBottom: 4 }} />
    <Stack tokens={{ childrenGap: 8, padding: 16 }} style={{ width: '100%', overflowY: 'auto' }}>
      {local && control && (
        <>
          <TextController label="Name" name="name" control={control} readOnly />
          <DefaultValueController name="state" control={control} />
          <DefaultValueController name="recycleRequestAt" control={control} />
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
          <ToggleController
            label="Enable emulation on Win Arm64"
            name="enableEmulationOnWinArm64"
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
              // eslint-disable-next-line @typescript-eslint/no-explicit-any
              name={`recycling.periodicRestart.schedule.${index}.time` as any}
              onRenderSuffix={() => <IconButton iconProps={{ iconName: 'clear' }} onClick={() => removeSchedule(index)}></IconButton>}
              control={control}
            />
          )}
          <DefaultValueController name="workerProcesses" control={control} />
          <StackItem>
            <DefaultButton iconProps={{ iconName: 'Add' }} onClick={() => appendSchedule({ time: '00:00:00' })}>Add Schedule</DefaultButton>
          </StackItem>

        </>
      )}
    </Stack>
  </Stack>
}