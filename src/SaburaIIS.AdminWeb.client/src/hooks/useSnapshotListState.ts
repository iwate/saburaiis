import useSWR from "swr";
import { getSnapshots } from "../api";

export type ISnapshotListState = string[]

export type ISnapshotListStateActions = {
  refresh: () => void
}

function fetcher([,partitionName, scaleSetName, instanceName]:[string, string | null, string | null, string | null]) {
  if (!partitionName || !scaleSetName || !instanceName) {
    return Promise.resolve(undefined);
  }
  return getSnapshots(partitionName, scaleSetName, instanceName);
}

export default function useSnapshotListState(partitionName: string|undefined, scaleSetName: string|undefined, instanceName: string|undefined): [state: ISnapshotListState | undefined, error: string|null, actions: ISnapshotListStateActions] {
  const {data, error, mutate} = useSWR([`partitions/${partitionName}/instances/${scaleSetName}/${instanceName}/snapshots`, partitionName, scaleSetName, instanceName], fetcher);
  const refresh = () => {
    mutate()
  }

  return [data, error, { refresh }]
}