import { CommandBar, Stack } from "@fluentui/react"
import { useForm } from "react-hook-form";
import { useHistory, useParams } from "react-router";
import { DEFAULT_SITE } from "../api";
import { DropdownController, TextController } from "../parts/controllers";
import { useBreadcrumb } from "../shared/Breadcrumb";
import { usePartitionState } from "../state";

export const SiteCreate = () => {
  const browserHistory = useHistory();
  const { partitionName } = useParams();
  const { local, setLocal } = usePartitionState(partitionName);

  const { handleSubmit, control, formState: { isDirty } } = useForm({
    mode: 'onChange',
    defaultValues: {
      name: '',
      applicationPoolName: local?.applicationPools[0]?.name
    },
  });

  useBreadcrumb([
    { text: partitionName, key: `/partitions/${partitionName}`, href: `/partitions/${partitionName}` },
    { text: 'Sites', key: `/partitions/${partitionName}/sites/` },
    { text: 'New', key: `/partitions/${partitionName}/sites/@new` },
  ]);

  if (local === null) {
    return <div>Loading...</div>
  }

  const onSubmit = (data) => {
    console.log(data)
    setLocal({
      ...local,
      sites:[
        ...local.sites,
        {
            ...DEFAULT_SITE,
            name: data.name,
            applicationPoolName: data.applicationPoolName
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

  const applicationPoolOptions = local.applicationPools.map(pool => ({
    key: pool.name,
    text: pool.name,
  }))
  return <Stack style={{ width: '100%' }}>
    <CommandBar items={commands} style={{ borderBottom: '1px solid #eee', paddingBottom: 4 }} />
    <Stack tokens={{ childrenGap: 8, padding: 16 }} style={{ width: '100%', overflowY: 'auto' }}>
      <TextController 
        label="Name" 
        name="name" 
        control={control}
      />
      <DropdownController
        label="Application Pool"
        name="applicationPoolName"
        options={applicationPoolOptions}
        control={control}
      />
    </Stack>
  </Stack>
}