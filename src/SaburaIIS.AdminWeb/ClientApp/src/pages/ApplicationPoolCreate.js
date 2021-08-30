import { CommandBar, Stack } from "@fluentui/react"
import { useForm } from "react-hook-form";
import { useHistory, useParams } from "react-router";
import { DEFAULT_APPLICATION_POOL } from "../api";
import { DropdownController, TextController } from "../parts/controllers";
import { useBreadcrumb } from "../shared/Breadcrumb";
import { usePartitionState } from "../state";

export const ApplicationPoolCreate = () => {
  const browserHistory = useHistory();
  const { partitionName } = useParams();
  const { local, setLocal } = usePartitionState(partitionName);

  const { handleSubmit, control, formState: { isDirty } } = useForm({
    mode: 'onChange',
    defaultValues: {
      name: '',
      source: local?.applicationPools[0]?.name
    },
  });

  useBreadcrumb([
    { text: partitionName, key: `/partitions/${partitionName}`, href: `/partitions/${partitionName}` },
    { text: 'Application Pools', key: `/partitions/${partitionName}/apppools/` },
    { text: 'New', key: `/partitions/${partitionName}/apppools/@new` },
  ]);

  if (local === null) {
    return <div>Loading...</div>
  }

  const onSubmit = (data) => {
    console.log(data)
    setLocal({
      ...local,
      applicationPools:[
        ...local.applicationPools,
        {
          ...(local.applicationPools.find(pool => pool.name === data.source)||DEFAULT_APPLICATION_POOL),
          name: data.name
        }
      ]
    });
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
      <TextController 
        label="Name" 
        name="name" 
        control={control}
      />
      <DropdownController 
        label="Clone source" 
        name="source" 
        options={local.applicationPools.map(pool => ({ key: pool.name, text: pool.name}))}
        control={control}
      />
    </Stack>
  </Stack>
}