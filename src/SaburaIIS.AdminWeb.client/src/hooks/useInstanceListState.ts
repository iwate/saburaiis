import useSWR from "swr";
import { getInstances } from "../api";
import { useMemo } from "react";
import { objectStateOptions } from "../constants";

export type IInstance = {
  scaleSetName: string,
  instanceName: string,
  siteName: string,
  siteState: string,
  appPath: string,
  vdirPath: string,
  vdirPhysicalPath: string,
  apppoolName: string,
  apppoolState: string,
}

export type IInstancesActions = {
  refresh: () => void
}

function fetcher([,partitionName, etag]:[string, string | undefined, string | null | undefined]) {
  if (!partitionName || !etag) {
    return Promise.resolve(undefined);
  }
  return getInstances(partitionName, etag);
}

export default function useInstanceListState(partitionName: string|undefined, etag: string|null|undefined): [state: IInstance[] | undefined, error: string|null, actions: IInstancesActions] {
  const {data, error, mutate} = useSWR([`partitions/${partitionName}`, partitionName, etag], fetcher);
  const refresh = () => {
    mutate();
  }
  const incetances = useMemo(() => data?.reduce((a,b) => {
    return a.concat(b.current.sites.reduce((c,d) => {
      return c.concat(d.applications.reduce((e,f) => {
        const apppool = b.current.applicationPools.find(pool => pool.name === f.applicationPoolName);
        return e.concat(f.virtualDirectories.map(vdir => ({
          scaleSetName: b.scaleSetName,
          instanceName: b.name,
          siteName: d.name,
          siteState: objectStateOptions.find(item => item.key === d.state)?.text ?? '',
          appPath: f.path,
          vdirPath: vdir.path,
          vdirPhysicalPath: vdir.physicalPath,
          apppoolName: apppool?.name ?? '',
          apppoolState: objectStateOptions.find(item => item.key === apppool?.state)?.text ?? '',
        })))
      }, [] as IInstance[]))
    }, [] as IInstance[]))
  }, [] as IInstance[]) ?? [], [data])
  
  return [incetances, error, {refresh}]
}