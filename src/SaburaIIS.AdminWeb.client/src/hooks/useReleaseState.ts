import useSWR from "swr";
import { getRelease } from "../api";
import { IRelease } from "../type";

function fetcher([,packageName, versionName]:[string, string | null, string | null]) {
  if (!packageName || !versionName) {
    return Promise.resolve(undefined);
  }
  return getRelease(packageName, versionName);
}

export default function useReleaseState(packageName: string|null, versionName: string|null): [state: IRelease | undefined, error: string|null] {
  const {data, error} = useSWR([`packages/${packageName}/releases/${versionName}`, packageName, versionName], fetcher);

  return [data, error]
}