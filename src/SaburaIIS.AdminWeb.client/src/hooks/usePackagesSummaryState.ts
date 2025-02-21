import useSWR from "swr";
import { getPackageSummary } from "../api";
import { IPackagesSummary } from "../type";

export type IPackagesSummaryStateActions = {
  set: (local: IPackagesSummary) => void
  refresh: () => void
}

function fetcher() {
  return getPackageSummary();
}

export default function usePackagesSummaryState(): [state: IPackagesSummary | undefined, error: string|null, actions: IPackagesSummaryStateActions] {
  const {data, error, mutate} = useSWR('packages', fetcher);
  const set = (state: IPackagesSummary) => {
    mutate(state);
  }
  const refresh = () => {
    mutate(data);
  }

  return [data, error, { set, refresh }]
}