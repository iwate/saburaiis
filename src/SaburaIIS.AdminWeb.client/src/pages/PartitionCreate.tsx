import { MessageBar, MessageBarType, PrimaryButton, Stack, StackItem } from "@fluentui/react"
import { useForm } from "react-hook-form";
import { addPartition } from "../api";
import { useNavigate } from "react-router";
import usePartitionListState from "../hooks/usePartitionListState";
import useErrorsState from "../hooks/useErrorsState";
import { useBreadcrumbs } from "../assemblies/Breadcrumbs";
import { errorMessage } from "../helper";
import { TextController } from "../assemblies/Controllers";

export default function PartitionCreate() {
  const navigate = useNavigate();
  const [partitions, loadError, { refresh }] = usePartitionListState();
  const [errors, setErrors] = useErrorsState(loadError);
  const { handleSubmit, control, formState: { isDirty, isValid } } = useForm({
    mode: 'onChange',
    defaultValues: {
      name: '',
    },
  });

  useBreadcrumbs([
    { text: 'New Partition', key: `/partitions/` },
  ]);

  const onSubmit: Parameters<typeof handleSubmit>[0] = async (data) => {
    try {
      await addPartition(data.name);
      refresh();
      navigate(`/partitions/${data.name}`);
    } catch (err) {
      setErrors([...errors, errorMessage(err)]);
    }
  }

  const validate = (value: string) => {
    return !partitions?.includes(value) || `'${value}' is already exists.`;
  }

  return <Stack style={{ width: '100%' }}>
    <Stack tokens={{ childrenGap: 8, padding: 16 }} style={{ width: '100%', overflowY: 'auto' }}>
      {errors.map((err, index) => (
        <MessageBar
          key={index}
          onDismiss={() => setErrors([...errors.slice(0, index), ...errors.slice(index + 1)])}
          messageBarType={MessageBarType.error}
          isMultiline={false}
          dismissButtonAriaLabel="Close"
        >{err}</MessageBar>
      ))}
      <TextController
        label="Name"
        name="name"
        required
        validate={validate}
        control={control}
      />
      <StackItem>
        <PrimaryButton
          disabled={!isDirty || !isValid}
          onClick={() => handleSubmit(onSubmit)()}
        >Create</PrimaryButton>
      </StackItem>
    </Stack>
  </Stack>
}