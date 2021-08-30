import { useEffect, useMemo, useState } from 'react';
import { useParams } from 'react-router-dom';
import { useBreadcrumb } from '../shared/Breadcrumb';
import { getInstance, getSnapshots, getSnapshot } from '../api'
import { DetailsList, Stack, Selection, SelectionMode, StackItem } from '@fluentui/react';
import { MonacoDiffEditor } from 'react-monaco-editor';
import { serializeToJson } from '../helper';
export const History = () => {
  const { partitionName, scaleSetName, instanceName } = useParams();
  useBreadcrumb([
    { text: partitionName, key: `/partitions/${partitionName}`, href: `/partitions/${partitionName}` },
    { text: 'Instances', key: `/partitions/${partitionName}/instances/`, href: `/partitions/${partitionName}/instances/` },
    { text: `${scaleSetName} / ${instanceName}` }
  ])
  const [instance, setInstance] = useState(null);
  const [snapshots, setSnapshots] = useState([]);
  const [selected, setSelected] = useState([]);

  const [left, setLeft] = useState(null);
  const [right, setRight] = useState(null);

  useEffect(() => {
    (async () => {
      setInstance(await getInstance(partitionName, scaleSetName, instanceName));
      setSnapshots(await getSnapshots(partitionName, scaleSetName, instanceName));
    })()
  }, [partitionName, scaleSetName, instanceName])

  const getDetail = async (item) => {
    const value = (item.timestamp === instance?.current.timestamp) ? instance?.current
                : await getSnapshot(partitionName, scaleSetName, instanceName, item.timestamp);

    return { label: item.text, value };
  }

  useEffect(() => {
    (async () => {
      const [r, l] = selected;
      setLeft(l ? await getDetail(l) : null);
      setRight(r ? await getDetail(r) : null);
    })()
  }, [selected])

  const detailList = useMemo(() => {
    const columns = [
      {
        key: 'timestamp',
        name: 'Timestamp',
        fieldName: 'text',
        minWidth: 200,
      },
    ]

    const formatter = new Intl.DateTimeFormat('default-arab-iso8601', {
      year: 'numeric', month: 'numeric', day: 'numeric',
      hour: 'numeric', minute: 'numeric', second: 'numeric',
      fractionalSecondDigits: 3
    });

    const items = [
      ...snapshots.map(timestamp => ({ timestamp, text: formatter.format(new Date(timestamp)) }))
    ];

    if (instance)
      items.unshift({ timestamp: instance.current.timestamp, text: 'Current' });

    const selection = new Selection({
      onSelectionChanged: function () {
        this._selected = this._selected || [];
        if (selection.getSelectedCount() <= 2) {
          this._selected = selection.getSelectedIndices();
          setSelected(selection.getSelection());
        }
        else {
          for (let index of this._selected.slice(1)) {
            selection.setIndexSelected(index, false);
            break;
          }
        }
      },
      selectionMode: SelectionMode.multiple,
    });

    return <DetailsList
      columns={columns}
      items={items}
      selection={selection}
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
  }, [instance, snapshots])

  if (instance == null)
    return <div>Loading...</div>

  return <Stack horizontal verticalFill>
    {detailList}
    <Stack grow>
      <Stack horizontal>
        <StackItem grow style={{ textAlign: 'center' }}><label>{left?.label || 'Not selected'}</label></StackItem>
        <StackItem grow style={{ textAlign: 'center' }}><label>{right?.label || 'Not selected'}</label></StackItem>
      </Stack>
      <StackItem grow>
        <MonacoDiffEditor
          language="json"
          original={left ? serializeToJson(left.value) : ''}
          value={right ? serializeToJson(right.value) : ''}
            options={{ readOnly: true }} />
      </StackItem>
    </Stack>
  </Stack>
}