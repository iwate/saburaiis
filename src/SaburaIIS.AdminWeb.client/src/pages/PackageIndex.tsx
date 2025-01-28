import { CommandBar, DefaultButton, Dialog, DialogFooter, ICommandBarItemProps, PrimaryButton, Stack } from "@fluentui/react";
import { useBoolean } from "@fluentui/react-hooks";
import { useParams } from "react-router";
import { removePackage } from "../api";

type RouteParams = {
  packageName: string
}

export default function PackageIndex() {
  const { packageName } = useParams<RouteParams>();
  const [hiddenDeleteDialog, { toggle: toggleHiddenDeleteDialog }] = useBoolean(true);

  const deletePartition = async () => {
    if (packageName) {
      await removePackage(packageName);
    }
    location.href = '/';
  }
  
  const commands: ICommandBarItemProps[] = [{
    key: 'delete',
    text: 'Delete package',
    iconProps: { iconName: 'Delete' },
    onClick: toggleHiddenDeleteDialog,
    split: true,
  }];
  return (
    <>
      <Stack grow style={{ width: '100%', overflowY: 'auto' }}>
        <CommandBar items={commands} style={{ borderBottom: '1px solid #eee', paddingBottom: 4 }} />
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
              >Yes, I delete this package</DefaultButton>
              <PrimaryButton onClick={toggleHiddenDeleteDialog}>Cancel</PrimaryButton>
            </DialogFooter>
          </Dialog>
    </>
  )
}