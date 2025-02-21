import { MessageBar, MessageBarType, Stack, TextField } from "@fluentui/react";
import { useParams } from "react-router"
import { useBreadcrumbs } from "../assemblies/Breadcrumbs";
import useReleaseState from "../hooks/useReleaseState";
import useErrorsState from "../hooks/useErrorsState";

type RouteParams = {
  packageName: string
  versionName: string
}
export default function Release() {
  const { packageName, versionName } = useParams<RouteParams>();
  const [release, loadError] = useReleaseState(packageName!, versionName!);
  const [errors, setErrors] = useErrorsState(loadError);

  useBreadcrumbs([
    { text: 'Packages', key: `/packages/` },
    { text: packageName!, key: `/packages/${packageName}`, href: `/packages/${packageName}` },
    { text: 'Releases', key: `/packages/${packageName}/releases` },
    { text: versionName!, key: `/packages/${packageName}/releases/${versionName}` },
  ]);
  
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
      {release && (
        <>
          <TextField label="Version" value={release.version} readOnly />
          <TextField label="Url" value={release.url} readOnly />
          <TextField label="Released At" value={new Date(release.releaseAt).toLocaleString()} readOnly />
        </>
      ) }
    </Stack>
  </Stack>
}