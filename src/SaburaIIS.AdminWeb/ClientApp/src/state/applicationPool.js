import { selectorFamily, useRecoilState, useRecoilValue } from "recoil";
import { originState as partitionOrigin, localState as partitionLocal } from "./partition";


const originState = selectorFamily({
  key: 'origin/applicationPool',
  get: ({partitionName, applicationPoolName}) => ({get}) => {
    const partition = get(partitionOrigin({partitionName}));
    return partition?.applicationPools.find(ap => ap.name === applicationPoolName);
  }
})

const localState = selectorFamily({
  key: 'local/applicationPool',
  get: ({partitionName, applicationPoolName}) => ({get}) => {
    const partition = get(partitionLocal({partitionName}));
    return partition?.applicationPools.find(ap => ap.name === applicationPoolName);
  },
  set: ({partitionName, applicationPoolName}) => ({get,set}, newValue) => {
    const partition = get(partitionLocal({partitionName}));
    const index = partition.applicationPools.findIndex(p => p.name === applicationPoolName);
    if (index !== -1) {
      const next = { ...partition };
      if (!newValue.recycling.periodicRestart.schedule)
        newValue.recycling.periodicRestart.schedule = [];
      next.applicationPools = [...partition.applicationPools.slice(0,index), newValue, ...partition.applicationPools.slice(index+1)]
      set(partitionLocal({partitionName}), next);
    }
  }
})

export const useApplicationPoolState = (partitionName, applicationPoolName) => {
  const origin = useRecoilValue(originState({partitionName, applicationPoolName}));
  const [local, setLocal] = useRecoilState(localState({partitionName, applicationPoolName}));

  return {
    origin,
    local, setLocal,
  }
}