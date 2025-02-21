import useSWR from "swr";
import { getReleaseVersions } from "../api";

export type IReleaseListState = string[]

export type IReleaseListStateActions = {
  set: (local: IReleaseListState) => void
  refresh: () => void
}

function fetcher([,packageName]:[string, string | null]) {
  if (!packageName) {
    return Promise.resolve([]);
  }
  return getReleaseVersions(packageName);
}

export default function useReleaseListState(packageName: string|null): [state: IReleaseListState | undefined, error: string|null, actions: IReleaseListStateActions] {
  const {data, error, mutate} = useSWR([`packages/${packageName}/releases`, packageName], fetcher);
  const set = (state: IReleaseListState) => {
    mutate(state);
  }
  const refresh = () => {
    mutate(data);
  }

  return [data, error, { set, refresh }]
}