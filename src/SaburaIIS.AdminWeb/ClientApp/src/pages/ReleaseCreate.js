import { MessageBar, MessageBarType, PrimaryButton, Stack, StackItem } from "@fluentui/react"
import { useState } from "react";
import { useForm } from "react-hook-form";
import { useHistory, useParams } from "react-router";
import { addRelease, getReleaseVersions } from "../api";
import { TextController } from "../parts/controllers";
import { useBreadcrumb } from "../shared/Breadcrumb";
import { useReleasesState } from "../state";

export const ReleaseCreate = () => {
  const browserHistory = useHistory();
  const { packageName } = useParams();
  const [ releases, setReleases] = useReleasesState();
  const { handleSubmit, control, formState: { isDirty, isValid } } = useForm({
    mode: 'onChange',
    defaultValues: {
      version: '',
      url: ''
    },
  });

  const [errors, setErrors] = useState([]);

  useBreadcrumb([
    { text: 'Packages', key: `/packages/` },
    { text: 'Packages', key: `/packages/${packageName}`, href: `/packages/${packageName}` },
    { text: 'Releases', key: `/packages/${packageName}/releases` },
    { text: 'New', key: `/packages/${packageName}/releases/@new` },
  ]);

  const onSubmit = async (data) => {
    try {
      await addRelease(packageName, data.version, data.url);
      const versions = await getReleaseVersions(packageName);
      setReleases(versions);
      browserHistory.push(`/packages/${packageName}`);
    } catch(err) {
      setErrors([...errors, err]);
    }
  }

  const validate = value => {
    return !!releases.indexOf(value) || `'${value}' is already exists.`;
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