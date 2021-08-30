import { selectorFamily, useRecoilState, useRecoilValue } from "recoil";
import { originState as partitionOrigin, localState as partitionLocal } from "./partition";


const originState = selectorFamily({
  key: 'origin/site',
  get: ({partitionName, siteName}) => ({get}) => {
    const partition = get(partitionOrigin({partitionName}));
    return partition?.sites.find(site => site.name === siteName);
  }
})

const localState = selectorFamily({
  key: 'local/site',
  get: ({partitionName, siteName}) => ({get}) => {
    const partition = get(partitionLocal({partitionName}));
    return partition?.sites.find(site => site.name === siteName);
  },
  set: ({partitionName, siteName}) => ({get,set}, newValue) => {
    const partition = get(partitionLocal({partitionName}));
    const index = partition.sites.findIndex(site => site.name === siteName);
    if (index !== -1) {
      const next = { ...partition };
      if (!newValue.applications)
        newValue.applications = [];
      if (!newValue.bindings)
        newValue.bindings = [];
      for (let app of newValue.applications)
        if (!app.virtualDirectories)
          app.virtualDirectories = [];
      next.sites = [...partition.sites.slice(0,index), newValue, ...partition.sites.slice(index+1)]
      set(partitionLocal({partitionName}), next);
    }
  }
})

export const useSiteState = (partitionName, siteName) => {
  const origin = useRecoilValue(originState({partitionName, siteName}));
  const [local, setLocal] = useRecoilState(localState({partitionName, siteName}));

  return {
    origin,
    local, setLocal,
  }
}