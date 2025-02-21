import { MessageBar, MessageBarType, PrimaryButton, Stack, StackItem } from "@fluentui/react"
import { useForm } from "react-hook-form";
import { addPackage } from "../api";
import { useBreadcrumbs } from "../assemblies/Breadcrumbs";
import usePackagesSummaryState from "../hooks/usePackagesSummaryState";
import { useNavigate } from "react-router";
import { TextController } from "../assemblies/Controllers";
import useErrorsState from "../hooks/useErrorsState";
import { errorMessage } from "../helper";

export default function PackageCreate() {
  useBreadcrumbs([
    { text: 'New Package', key: `/packages/` },
  ]);
  const navigate = useNavigate();
  const [, loadError, { refresh }] = usePackagesSummaryState();
  const { handleSubmit, control, formState: { isDirty, isValid } } = useForm({
    mode: 'onChange',
    defaultValues: {
      name: '',
    },
  });

  const [errors, setErrors] = useErrorsState(loadError);

  const onSubmit: Parameters<typeof handleSubmit>[0] = async (data) => {
    try {
      await addPackage(data.name);
      refresh();
      navigate(`/packages/${data.name}`);
    } catch (err) {
      setErrors([...errors, errorMessage(err)]);
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