import { MessageBar, MessageBarType, PrimaryButton, Stack, StackItem } from "@fluentui/react"
import { useState } from "react";
import { useForm } from "react-hook-form";
import { useHistory } from "react-router";
import { addPartition } from "../api";
import { TextController } from "../parts/controllers";
import { useBreadcrumb } from "../shared/Breadcrumb";
import { usePartitionListState } from "../state";

export const PartitionCreate = () => {
  const browserHistory = useHistory();
  const { setLocal } = usePartitionListState();
  const { handleSubmit, control, formState: { isDirty, isValid } } = useForm({
    mode: 'onChange',
    defaultValues: {
      name: '',
    },
  });

  const [errors, setErrors] = useState([]);

  useBreadcrumb([
    { text: 'New Partition', key: `/partitions/` },
  ]);

  const onSubmit = (data) => {
    try {
      addPartition(data.name);
      setLocal(origin => [...origin, { name: data.name, hasDiff: false }]);
      browserHistory.push(`/`);
    } catch (err) {
      setErrors([...errors, err]);
    }
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