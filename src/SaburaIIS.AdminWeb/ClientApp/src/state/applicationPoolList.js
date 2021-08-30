import { selectorFamily, useRecoilValue } from "recoil";
import { originState as partitionOrigin, localState as partitionLocal } from "./partition";


const originState = selectorFamily({
  key: 'origin/applicationPools',
  get: ({partitionName}) => ({get}) => {
    const partition = get(partitionOrigin({partitionName}));
    return partition?.applicationPools.map(ap => ap.Name);
  }
})

const localState = selectorFamily({
  key: 'local/applicationPools',
  get: ({partitionName}) => ({get}) => {
    const partition = get(partitionLocal({partitionName}));
    return partition?.applicationPools.map(ap => ap.Name);
  }
})

export const useApplicationPoolListValue = (partitionName) => {
  const origin = useRecoilValue(originState({partitionName}));
  const local = useRecoilValue(localState({partitionName}));

  return {
    origin,
    local,
  }
}