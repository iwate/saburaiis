import { DetailsList, Stack, Link, CommandBar, SelectionMode, MessageBar, MessageBarType } from "@fluentui/react";
import { useNavigate, useParams } from "react-router";
import usePartitionState from "../hooks/usePartitionState";
import { useBreadcrumbs } from "../assemblies/Breadcrumbs";
import useErrorsState from "../hooks/useErrorsState";
import useInstanceListState, { IInstance } from "../hooks/useInstanceListState";

type RouteParams = {
    partitionName: string
}

export default function InstancesIndex() {
  const navigate = useNavigate();
  const { partitionName } = useParams<RouteParams>();
  const [origin, loadError1] = usePartitionState(partitionName);
  const [instances, loadError2, { refresh }] = useInstanceListState(partitionName, origin?.["@etag"]);
  const [errors, setErrors] = useErrorsState(loadError1, loadError2);

  useBreadcrumbs([
    { text: partitionName!, key: `/partitions/${partitionName}`, href: `/partitions/${partitionName}` },
    { text: 'Instances', key: `/partitions/${partitionName}/instances` },
  ]);

  const columns = [
    {
      key: 'scaleSetName',
      name: 'Scale Set',
      fieldName: 'scaleSetName',
      isResizable: true,
      minWidth: 32, maxWidth: 64,
    },
    {
      key: 'instanceName',
      name: 'Instance',
      fieldName: 'instanceName',
      flexGrow: 1,
      isResizable: true,
      minWidth: 32, maxWidth: 64,
      onRender: (item: IInstance) => (
        <Link
          key={`${item.scaleSetName}-${item.instanceName}`}
          onClick={() => navigate(`/partitions/${partitionName}/instances/${item.scaleSetName}/${item.instanceName}`)}>
          {item.instanceName}
        </Link>
      )
    },
    {
      key: 'siteName',
      name: 'Site',
      fieldName: 'siteName',
      flexGrow: 1,
      isResizable: true,
      minWidth: 64, maxWidth: 128,
    },
    {
      key: 'apppoolName',
      name: 'Application Pool',
      fieldName: 'apppoolName',
      flexGrow: 1,
      isResizable: true,
      minWidth: 64, maxWidth: 128,
    },
    {
      key: 'appPath',
      name: 'Application',
      fieldName: 'appPath',
      flexGrow: 1,
      isResizable: true,
      minWidth: 64, maxWidth: 96,
    },
    {
      key: 'vdirPath',
      name: 'Virtual Directory',
      fieldName: 'vdirPath',
      flexGrow: 1,
      isResizable: true,
      minWidth: 64, maxWidth: 128,
    },
    {
      key: 'vdirPhysicalPath',
      name: 'Path',
      fieldName: 'vdirPhysicalPath',
      flexGrow: 1,
      isResizable: true,
      minWidth: 200, maxWidth: 300,
    },
    {
      key: 'siteState',
      name: 'Site State',
      fieldName: 'siteState',
      flexGrow: 1,
      isResizable: true,
      minWidth: 32, maxWidth: 64,
    },
    {
      key: 'apppoolState',
      name: 'Application Pool State',
      fieldName: 'apppoolState',
      flexGrow: 1,
      isResizable: true,
      minWidth: 64, maxWidth: 128,
    }
  ];

  const commands = [
    {
      key: 'reload',
      text: 'Reload',
      iconProps: { iconName: 'Refresh' },
      onClick: refresh,
    }
  ]
  return (
    <Stack grow style={{ width: '100%', overflowY: 'auto' }}>
      {errors.map((err, index) => (
        <MessageBar
          key={index}
          onDismiss={() => setErrors([...errors.slice(0, index), ...errors.slice(index + 1)])}
          messageBarType={MessageBarType.error}
          isMultiline={false}
          dismissButtonAriaLabel="Close"
        >{err}</MessageBar>
      ))}
      <CommandBar items={commands} style={{ borderBottom: '1px solid #eee', paddingBottom: 4 }} />
      {instances && (
        <DetailsList
        items={instances}
        columns={columns}
        selectionMode={SelectionMode.none}
      />
      )}
    </Stack>
  )
}