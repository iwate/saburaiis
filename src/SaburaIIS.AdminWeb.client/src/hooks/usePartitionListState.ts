import useSWR from "swr";
import { getPartitionNames } from "../api";

export type IPartitionListStateActions = {
  refresh: () => void
}

export default function usePartitionListState(): [state: string[] | undefined, error: string|null, actions: IPartitionListStateActions] {
  const {data, error, mutate} = useSWR('partitions', getPartitionNames);
  const refresh = () => {
    mutate(data)
  }

  return [data, error, { refresh }]
}