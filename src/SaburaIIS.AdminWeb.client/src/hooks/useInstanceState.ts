import useSWR from "swr";
import { getInstance } from "../api";
import { IVirtualMachine } from "../type";

export type IInstanceStateActions = {
  refresh: () => void
}

function fetcher([,partitionName, scaleSetName, instanceName]:[string, string | null, string | null, string | null]) {
  if (!partitionName || !scaleSetName || !instanceName) {
    return Promise.resolve(undefined);
  }
  return getInstance(partitionName, scaleSetName, instanceName);
}

export default function useInstanceState(partitionName: string|undefined, scaleSetName: string|undefined, instanceName: string|undefined): [state: IVirtualMachine | undefined, error: string|null, actions: IInstanceStateActions] {
  const {data, error, mutate} = useSWR([`partitions/${partitionName}/instances/${scaleSetName}/${instanceName}`, partitionName, scaleSetName, instanceName], fetcher);
  const refresh = () => {
    mutate()
  }

  return [data, error, { refresh }]
}