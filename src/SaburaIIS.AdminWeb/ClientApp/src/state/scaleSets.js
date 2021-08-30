import { selectorFamily, useRecoilState, useRecoilValue } from "recoil";
import { originState as partitionOrigin, localState as partitionLocal } from "./partition";


const originState = selectorFamily({
  key: 'origin/applicationPool',
  get: ({partitionName}) => ({get}) => {
    const partition = get(partitionOrigin({partitionName}));
    return {
        ScaleSets: partition?.scaleSets
    }
  }
})

const localState = selectorFamily({
  key: 'local/applicationPool',
  get: ({partitionName}) => ({get}) => {
    const partition = get(partitionLocal({partitionName}));
    return {
      scaleSets: partition?.scaleSets
    }
  },
  set: ({partitionName}) => ({get,set}, newValue) => {
    const partition = get(partitionLocal({partitionName}));
    const next = {...partition};
    next.scaleSets = newValue.scaleSets || [];
    set(partitionLocal({partitionName}), next);
  }
})

export const useScaleSetsState = (partitionName) => {
  const origin = useRecoilValue(originState({partitionName}));
  const [local, setLocal] = useRecoilState(localState({partitionName}));

  return {
    origin,
    local, setLocal,
  }
}