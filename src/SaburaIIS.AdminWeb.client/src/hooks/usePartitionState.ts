import useSWR from "swr";
import { getPartition } from "../api";
import { IPartition } from "../type";

export type IPartitionStateActions = {
  refresh: () => void
}

function fetcher([,partitionName]:[string, string | null]) {
  if (!partitionName) {
    return Promise.resolve(undefined);
  }
  return getPartition(partitionName);
}

export default function usePartitionState(partitionName: string|undefined): [state: IPartition | undefined, error: string|null, actions: IPartitionStateActions] {
  const {data, error, mutate} = useSWR([`partitions/${partitionName}`, partitionName], fetcher);
  const refresh = () => {
    mutate(data)
  }

  return [data, error, { refresh }]
}