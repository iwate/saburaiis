import { Stack } from "@fluentui/react";
import { useEffect, useState } from "react";
import { useParams } from "react-router"
import { getRelease } from "../api";
import { useBreadcrumb } from "../shared/Breadcrumb";

export const Release = () => {
  const { packageName, versionName } = useParams();
  const [release, setRelease] = useState();

  useBreadcrumb([
    { text: 'Packages', key: `/packages/` },
    { text: 'Packages', key: `/packages/${packageName}`, href: `/packages/${packageName}` },
    { text: 'Releases', key: `/packages/${packageName}/releases` },
    { text: versionName, key: `/packages/${packageName}/releases/${versionName}` },
  ]);

  useEffect(() => {
    (async () => {
      setRelease(await getRelease(packageName, versionName))
    })()
  }, [packageName, versionName]);

  if (release == null) {
    return <div>Loading...</div>
  }

  return <Stack style={{ width: '100%' }}>
    <Stack tokens={{ childrenGap: 8, padding: 16 }} style={{ width: '100%', overflowY: 'auto' }}>
      <h1>{versionName}</h1>
      <p>{release.url}</p>
    </Stack>
  </Stack>
}