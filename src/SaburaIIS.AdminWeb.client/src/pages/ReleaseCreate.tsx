import { MessageBar, MessageBarType, PrimaryButton, Stack, StackItem } from "@fluentui/react"
import { useForm } from "react-hook-form";
import { useNavigate, useParams } from "react-router";
import { addRelease } from "../api";
import useReleaseListState from "../hooks/useReleaseListState";
import { useBreadcrumbs } from "../assemblies/Breadcrumbs";
import useErrorsState from "../hooks/useErrorsState";
import { errorMessage } from "../helper";
import { TextController } from "../assemblies/Controllers";

type RouteParams = {
  packageName: string
}

export default function ReleaseCreate() {
  const navigate = useNavigate();
  const { packageName } = useParams<RouteParams>();
  const [releases, loadError, { refresh }] = useReleaseListState(packageName!);
  const { handleSubmit, control, formState: { isDirty, isValid } } = useForm({
    mode: 'onChange',
    defaultValues: {
      version: '',
      url: ''
    },
  });

  const [errors, setErrors] = useErrorsState(loadError);

  useBreadcrumbs([
    { text: 'Packages', key: `/packages/` },
    { text: 'Packages', key: `/packages/${packageName}`, href: `/packages/${packageName}` },
    { text: 'Releases', key: `/packages/${packageName}/releases` },
    { text: 'New', key: `/packages/${packageName}/releases/@new` },
  ]);

  const onSubmit: Parameters<typeof handleSubmit>[0] = async (data) => {
    try {
      await addRelease(packageName!, data.version, data.url);
      refresh();
      navigate(`/packages/${packageName}/releases/${data.version}`);
    } catch (err) {
      setErrors([...errors, errorMessage(err)]);
    }
  }

  const validate = (value: string) => {
    return !releases?.includes(value) || `'${value}' is already exists.`;
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
        label="Version"
        name="version"
        required
        validate={validate}
        control={control}
      />
      <TextController
        label="Url"
        name="url"
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