import { CommandBar, IconButton, Stack, TextField } from "@fluentui/react"
import { useEffect, useState } from "react";
import { useFieldArray, useForm } from "react-hook-form";
import { useHistory, useParams } from "react-router"
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

  const [value, setValue] = useState('');

  useEffect(() => {
    if (local !== null) {
      reset({
        ...local,
      });
    }
  }, [partitionName, local])

  useBreadcrumb([
    { text: partitionName, key: `/partitions/${partitionName}`, href: `/partitions/${partitionName}` },
    { text: 'ScaleSets', key: `/partitions/${partitionName}/scalesets/` },
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

        <TextField
          key={item.name}
          value={item.name}
          onRenderSuffix={() =>
            <IconButton
              iconProps={{ iconName: 'clear' }}
              onClick={() => removeScaleSet(index)}
            />}
          readOnly />
      )}
      <TextField
        value={value}
        onChange={(ev,newValue) => setValue(newValue)}
        onKeyUp={(ev) => {
          if (ev.key === 'Enter' || ev.keyCode === 13) {
            appendScaleSet({ name: value});
            setValue('');
          }
        }}
        onRenderSuffix={() => <IconButton iconProps={{ iconName: 'Add' }} onClick={() => {
          appendScaleSet({ name: value});
          setValue('');
        }}/>} />
    </Stack>
  </Stack>
}