import { CommandBar, ICommandBarItemProps, Stack } from "@fluentui/react"
import { useContext, useMemo } from "react";
import { useForm } from "react-hook-form";
import { useNavigate, useParams } from "react-router";
import { WorkingContext } from "../components/WorkingContext";
import { useBreadcrumbs } from "../assemblies/Breadcrumbs";
import { DEFAULT_SITE } from "../constants";
import { DropdownController, TextController } from "../assemblies/Controllers";

type RouteParams = {
  partitionName: string
}

export default function SiteCreat() {
  const navigate = useNavigate();
  const { partitionName } = useParams<RouteParams>();
  const { value: context, actions: { setPartition } } = useContext(WorkingContext);
  const local = useMemo(() => context.partitions[partitionName!], [context, partitionName]);

  const { handleSubmit, control, formState: { isDirty } } = useForm({
    mode: 'onChange',
    defaultValues: {
      name: '',
      applicationPoolName: local?.applicationPools[0]?.name
    },
  });

  useBreadcrumbs([
    { text: partitionName!, key: `/partitions/${partitionName}`, href: `/partitions/${partitionName}` },
    { text: 'Sites', key: `/partitions/${partitionName}/sites/` },
    { text: 'New', key: `/partitions/${partitionName}/sites/@new` },
  ]);

  const onSubmit: Parameters<typeof handleSubmit>[0] = (data) => {
    setPartition({
      ...local,
      sites: [
        ...local.sites,
        {
          ...DEFAULT_SITE,
          name: data.name,
          applications: [
            {
              ...DEFAULT_SITE.applications[0],
              applicationPoolName: data.applicationPoolName
            }
          ]
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

  const applicationPoolOptions = useMemo(() => local.applicationPools.map(pool => ({
    key: pool.name,
    text: pool.name,
  })), [local])

  const validate = (value: string) => {
    return !local?.sites.some(site => site.name === value) || `'${value}' is already exists.`;
  }
  return <Stack style={{ width: '100%' }}>
    <CommandBar items={commands} style={{ borderBottom: '1px solid #eee', paddingBottom: 4 }} />
    <Stack tokens={{ childrenGap: 8, padding: 16 }} style={{ width: '100%', overflowY: 'auto' }}>
      <TextController
        label="Name"
        name="name"
        validate={validate}
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