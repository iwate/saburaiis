import { CommandBar, DefaultButton, IconButton, Stack, StackItem, TextField } from "@fluentui/react"
import { useEffect } from "react";
import { useFieldArray, useForm } from "react-hook-form";
import { useHistory, useParams } from "react-router"
import { TextController } from "../parts/controllers";
import { useBreadcrumb } from "../shared/Breadcrumb";
import { useScaleSetsState } from "../state";

export const ScaleSets = () => {
  const browserHistory = useHistory();
  const { partitionName } = useParams();
  const { local, setLocal } = useScaleSetsState(partitionName);

  const { handleSubmit, control, formState: { isDirty }, reset } = useForm({
    mode: 'onChange',
    defaultValues: local,
  })

  const { fields: scaleSets, append: appendScaleSet, remove: removeScaleSet } = useFieldArray({
    control,
    name: 'scaleSets',
    keyName: 'name'
  });

  useEffect(() => {
    if (local !== null) {
      reset({
        ...local,
      });
    }
  }, [partitionName, local, reset])

  useBreadcrumb([
    { text: partitionName, key: `/partitions/${partitionName}`, href: `/partitions/${partitionName}` },
    { text: 'ScaleSets', key: `/partitions/${partitionName}/scalesets/` },
  ]);

  if (local === null) {
    return <div>Loading...</div>
  }

  const onSubmit = (data) => {
    setLocal(data);
    browserHistory.push(`/partitions/${partitionName}`);
  }

  const commands = [
    {
      key: 'stage',
      text: 'Stage',
      iconProps: { iconName: 'Add' },
      disabled: !isDirty,
      onClick: () => handleSubmit(onSubmit)(),
    }
  ]

  return <Stack style={{ width: '100%' }}>
    <CommandBar items={commands} style={{ borderBottom: '1px solid #eee', paddingBottom: 4 }} />
    <Stack tokens={{ childrenGap: 8, padding: 16 }} style={{ width: '100%', overflowY: 'auto' }}>
      {scaleSets.map((item, index) =>
        <TextController
          key={item.name}
          name={`scaleSets.${index}.name`}
          value={item.name}
          onRenderSuffix={() =>
            <IconButton
              iconProps={{ iconName: 'clear' }}
              onClick={() => removeScaleSet(index)}
            />}
          readOnly
          control={control} />
      )}
      <StackItem>
        <DefaultButton iconProps={{ iconName: 'Add' }} onClick={() => appendScaleSet({ name: '' })}>Add Scale Set</DefaultButton>
      </StackItem>
    </Stack>
  </Stack>
}