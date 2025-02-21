import { Stack, MessageBar, MessageBarType, CommandBar, Dialog, DialogFooter, DefaultButton, PrimaryButton, ICommandBarItemProps } from '@fluentui/react'
import { useBoolean } from '@fluentui/react-hooks'
import { useParams } from 'react-router';
import { useContext, useMemo } from 'react';
import { WorkingContext } from '../components/WorkingContext';
import usePartitionState from '../hooks/usePartitionState';
import { serializeToJson } from '../helper';
import useErrorsState from '../hooks/useErrorsState';
import { removePartition, replacePartition } from '../api';
import DiffViewer from '../components/DiffViewer';
import ErrorBoundary from '../components/ErrorBoundary';
import { useBreadcrumbs } from '../assemblies/Breadcrumbs';

type RouteParams = {
  partitionName: string
}

export default function PartitionIndex() {
  const { partitionName } = useParams<RouteParams>();

  useBreadcrumbs([
    { text: partitionName!, key: `/partitions/${partitionName}` },
  ]);

  const { value: context, actions: { setPartition } } = useContext(WorkingContext);
  const [origin, loadError, { refresh }] = usePartitionState(partitionName);
  const [errors, setErrors] = useErrorsState(loadError);
  const [hiddenDeleteDialog, { toggle: toggleHiddenDeleteDialog }] = useBoolean(true);
  const local = useMemo(() => context.partitions[partitionName!], [context.partitions, partitionName]);
  
  // useEffect(() => {
  //   if (origin && !local) {
  //     setPartition({ ...origin }, false);
  //   }
  // }, [origin, local]); // eslint-disable-line react-hooks/exhaustive-deps

  const commands = useMemo(() => {
    const commands: ICommandBarItemProps[] = [{
      key: 'delete',
      text: 'Delete Partition',
      iconProps: { iconName: 'Delete' },
      onClick: toggleHiddenDeleteDialog,
      split: true,
    }];

    if (context.diffs[partitionName!]) {
      commands.push(
        {
          key: 'apply',
          text: 'Apply',
          iconProps: { iconName: 'Checkmark' },
          onClick: () => {
            (async () => {
              const etag = await replacePartition(local);
              const data = { ...local, '@etag': etag };
              refresh();
              setPartition(data, false);
            })()
          },
          split: true,
        },
        {
          key: 'discard',
          text: 'Discard this changes',
          iconProps: { iconName: 'Undo' },
          onClick: () => { setPartition({ ...origin! }, false) },
        }
      )
    }

    return commands;
  }, [local]); // eslint-disable-line react-hooks/exhaustive-deps
  
  const deletePartition = async () => {
    await removePartition(origin!);
    location.href = '/';
  }

  return <>
    <Stack style={{ width: '100%' }}>
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
      <ErrorBoundary fallback="An error occured">
        <DiffViewer original={serializeToJson(origin??{})} modified={serializeToJson(context.partitions[partitionName!] ?? {})} />
      </ErrorBoundary>
    </Stack>
    <Dialog
      hidden={hiddenDeleteDialog}
      dialogContentProps={{
        subText: 'You cannot undo the deletion. Are you sure you want to delete this partition?',
        title: 'CAUTION'
      }}
      minWidth={348}
      onDismiss={toggleHiddenDeleteDialog}
    >
      <DialogFooter>
        <DefaultButton
          onClick={deletePartition}
        >Yes, I delete this partition</DefaultButton>
        <PrimaryButton onClick={toggleHiddenDeleteDialog}>Cancel</PrimaryButton>
      </DialogFooter>
    </Dialog>
  </>
}
