import { CommandBar, DefaultButton, ICommandBarItemProps, IconButton, Stack, StackItem } from "@fluentui/react"
import { useContext, useEffect, useMemo } from "react";
import { useFieldArray, useForm } from "react-hook-form";
import { useNavigate, useParams } from "react-router"
import { WorkingContext } from "../components/WorkingContext";
import { useBreadcrumbs } from "../assemblies/Breadcrumbs";
import { TextController } from "../assemblies/Controllers";

type RouteParams = {
  partitionName: string
}
export default function ScaleSets() {
  const navigate = useNavigate();
  const { partitionName } = useParams<RouteParams>();
  const { value: context, actions: { setPartition } } = useContext(WorkingContext);
  const local = useMemo(() => context.partitions[partitionName!], [context, partitionName]);

  const { handleSubmit, control, formState: { isDirty }, reset } = useForm({
    mode: 'onChange',
    defaultValues: local,
  })

  const { fields: scaleSets, append: appendScaleSet, remove: removeScaleSet } = useFieldArray({
    control,
    name: 'scaleSets',
  });

  useEffect(() => {
    if (local !== null) {
      reset({
        ...local,
      });
    }
  }, [partitionName, local, reset])

  useBreadcrumbs([
    { text: partitionName!, key: `/partitions/${partitionName}`, href: `/partitions/${partitionName}` },
    { text: 'ScaleSets', key: `/partitions/${partitionName}/scalesets/` },
  ]);

  const onSubmit: Parameters<typeof handleSubmit>[0] = (data) => {
    setPartition({
      ...local,
      scaleSets: [
        ...data.scaleSets
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

  return <Stack style={{ width: '100%' }}>
    <CommandBar items={commands} style={{ borderBottom: '1px solid #eee', paddingBottom: 4 }} />
    <Stack tokens={{ childrenGap: 8, padding: 16 }} style={{ width: '100%', overflowY: 'auto' }}>
      {local && (
        <>
          {scaleSets.map((_, index) =>
            <TextController
              key={index}
              name={`scaleSets.${index}.name`}
              onRenderSuffix={() =>
                <IconButton
                  iconProps={{ iconName: 'clear' }}
                  onClick={() => removeScaleSet(index)}
                />}
              // eslint-disable-next-line @typescript-eslint/no-explicit-any
              control={control as any} />
          )}
          <StackItem>
            <DefaultButton iconProps={{ iconName: 'Add' }} onClick={() => appendScaleSet({ name: '' })}>Add Scale Set</DefaultButton>
          </StackItem>
        </>
      )}
    </Stack>
  </Stack>
}