import { useParams } from "react-router"
import { useBreadcrumbs } from "../assemblies/Breadcrumbs"
import { Dispatch, SetStateAction, useEffect, useMemo, useState } from "react"
import { DetailsList, MessageBar, MessageBarType, Selection, SelectionMode, Stack, StackItem } from "@fluentui/react";
import DiffViewer from "../components/DiffViewer";
import { serializeToJson } from "../helper";
import useInstanceState from "../hooks/useInstanceState";
import useSnapshotListState from "../hooks/useSnapshotListState";
import useErrorsState from "../hooks/useErrorsState";
import { ISnapshot, IVirtualMachine } from "../type";
import { getSnapshot } from "../api";

const formatter = new Intl.DateTimeFormat('default-arab-iso8601', {
  year: 'numeric', month: 'numeric', day: 'numeric',
  hour: 'numeric', minute: 'numeric', second: 'numeric',
  // fractionalSecondDigits: 3
});

type RouteParams = {
  partitionName: string
  scaleSetName: string
  instanceName: string
}

type SnapshotPair = {
  left: { label: string, value: ISnapshot } | null
  right: { label: string, value: ISnapshot } | null
}

type SnapshotDetailsListItem = { timestamp: string, text: string }

async function getDetail(partitionName: string | undefined, instance: IVirtualMachine | undefined, item: SnapshotDetailsListItem ) {
  if (!partitionName || !instance) {
    return null;
  }

  return {
    label: item.text,
    value: (item.timestamp === instance?.current.timestamp) 
      ? instance?.current
      : await getSnapshot(partitionName, instance.scaleSetName, instance.name, item.timestamp)
  }
}

function useSelectedSnapshotPairState(partitionName: string | undefined, instance: IVirtualMachine|undefined): [value: SnapshotPair, set: Dispatch<SetStateAction<SnapshotDetailsListItem[]>> ] {
  const [selected, setSelected] = useState<SnapshotDetailsListItem[]>([]);
  const [value, setValue] = useState<SnapshotPair>({ left: null, right: null })

  useEffect(() => {
    (async () => {
      const [r, l] = selected;
      const left = l ? await getDetail(partitionName, instance, l) : null;
      const right = r ? await getDetail(partitionName, instance, r) : null;
      setValue({ left, right })
    })()
  }, [selected, instance, partitionName])
  
  return [value, setSelected]
}

export default function History() {
  const { partitionName, scaleSetName, instanceName } = useParams<RouteParams>();
  useBreadcrumbs([
    { text: partitionName!, key: `/partitions/${partitionName}`, href: `/partitions/${partitionName}` },
    { text: 'Instances', key: `/partitions/${partitionName}/instances/`, href: `/partitions/${partitionName}/instances/` },
    { text: `${scaleSetName} / ${instanceName}`, key: `/partitions/${partitionName}/instances/${scaleSetName}/${instanceName}` },
  ])
  const [instance, loadError1] = useInstanceState(partitionName, scaleSetName, instanceName);
  const [snapshots, loadError2] = useSnapshotListState(partitionName, scaleSetName, instanceName);
  const [errors, setErrors] = useErrorsState(loadError1, loadError2);
  const [pair, setSelected] = useSelectedSnapshotPairState(partitionName, instance);
  const columns = [
    {
      key: 'timestamp',
      name: 'Timestamp',
      fieldName: 'text',
      minWidth: 200,
    },
  ]
  const items = useMemo(() => {
    const items: SnapshotDetailsListItem[] = [];

    if (instance) {
      items.push({ timestamp: instance.current.timestamp, text: 'Current' });
    }

    if (snapshots) {
      items.push(...snapshots.map(timestamp => ({ timestamp, text: formatter.format(new Date(timestamp)) })))
    }

    return items;
  }, [instance, snapshots]) 

  // @ts-expect-error sample code allowed this style.
  const selection = new Selection<SnapshotDetailsListItem>({
    onSelectionChanged() {
      if (selection.getSelectedCount() <= 2) {
        setSelected(selection.getSelection());
      }
      else {
        const indeces = selection.getSelectedIndices();
        for (const index of indeces.slice(1)) {
          selection.setIndexSelected(index, false, false);
          break;
        }
      }
    },
    selectionMode: SelectionMode.multiple,
  });
  
  if (instance == null)
    return <div>Loading...</div>

  return <Stack horizontal verticalFill>
    {errors.map((err, index) => (
      <MessageBar
        key={index}
        onDismiss={() => setErrors([...errors.slice(0, index), ...errors.slice(index + 1)])}
        messageBarType={MessageBarType.error}
        isMultiline={false}
        dismissButtonAriaLabel="Close"
      >{err}</MessageBar>
    ))}
    <DetailsList
      columns={columns}
      items={items}
      // eslint-disable-next-line @typescript-eslint/no-explicit-any
      selection={selection as any}
      isHeaderVisible={false}
      selectionPreservedOnEmptyClick
      compact
      styles={{
        root: {
          width: 240,
          height: '100%',
          boxSizing: 'border-box',
          borderRight: '1px solid #eee',
          overflowY: 'auto',
          overflowX: 'hidden'
        },
      }}
    />
    <Stack grow>
      <Stack horizontal>
        <StackItem grow style={{ textAlign: 'center' }}><label>{pair.left?.label || 'Not selected'}</label></StackItem>
        <StackItem grow style={{ textAlign: 'center' }}><label>{pair.right?.label || 'Not selected'}</label></StackItem>
      </Stack>
      <StackItem grow>
        <DiffViewer
          original={pair.left ? serializeToJson(pair.left.value) : ''}
          modified={pair.right ? serializeToJson(pair.right.value) : ''}
        />
      </StackItem>
    </Stack>
  </Stack>
}