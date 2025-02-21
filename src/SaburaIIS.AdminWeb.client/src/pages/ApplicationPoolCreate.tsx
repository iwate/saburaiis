import { CommandBar, ICommandBarItemProps, Stack } from "@fluentui/react"
import { useForm } from "react-hook-form";
import { useNavigate, useParams } from "react-router";
import { DropdownController, TextController } from "../assemblies/Controllers";
import { useBreadcrumbs } from "../assemblies/Breadcrumbs";
import { useContext, useEffect, useMemo } from "react";
import { WorkingContext } from "../components/WorkingContext";
import { DEFAULT_APPLICATION_POOL } from "../constants";

type RouteParams = {
  partitionName: string
}

export default function ApplicationPoolCreate() {
  const navigate = useNavigate();
  const { partitionName } = useParams<RouteParams>();
  const { value: context, actions: { setPartition } } = useContext(WorkingContext);
  const local = useMemo(() => context.partitions[partitionName!], [context, partitionName]);

  const { handleSubmit, control, formState: { isDirty }, resetField } = useForm({
    mode: 'onChange',
    defaultValues: {
      name: '',
      source: local?.applicationPools[0]?.name
    },
  });

  useEffect(() => {
    resetField('source', { defaultValue: local?.applicationPools[0]?.name })
  }, [local]) // eslint-disable-line react-hooks/exhaustive-deps
  

  useBreadcrumbs([
    { text: partitionName!, key: `/partitions/${partitionName}`, href: `/partitions/${partitionName}` },
    { text: 'Application Pools', key: `/partitions/${partitionName}/apppools/` },
    { text: 'New', key: `/partitions/${partitionName}/apppools/@new` },
  ]);

  const onSubmit: Parameters<typeof handleSubmit>[0] = (data) => {
    setPartition({
      ...local,
      applicationPools: [
        ...local.applicationPools,
        {
          ...(local.applicationPools.find(pool => pool.name === data.source) ?? DEFAULT_APPLICATION_POOL),
          name: data.name
        }
      ]
    }, true);
    navigate(`/partitions/${partitionName}`);
  }

  const commands: ICommandBarItemProps[] = [
    {
      key: 'stage',
      text: 'Stage',
      iconProps: { iconName: 'Add' },
      disabled: !isDirty,
      onClick: () => { handleSubmit(onSubmit)() },
    }
  ]
  const validate = (value: string) => {
    return !local?.applicationPools.some(apppool => apppool.name === value) || `'${value}' is already exists.`;
  }
  return <Stack style={{ width: '100%' }}>
    <CommandBar items={commands} style={{ borderBottom: '1px solid #eee', paddingBottom: 4 }} />
    <Stack tokens={{ childrenGap: 8, padding: 16 }} style={{ width: '100%', overflowY: 'auto' }}>
      {local && (
        <>
          <TextController
            label="Name"
            name="name"
            required
            validate={validate}
            control={control}
          />
          <DropdownController
            label="Clone source"
            name="source"
            options={local.applicationPools.map(pool => ({ key: pool.name, text: pool.name }))}
            control={control}
          />
        </>
      ) }
    </Stack>
  </Stack>
}