import { atomFamily, useRecoilState } from "recoil"

export const originState = atomFamily({
  key: 'origin/partitions',
  default: null
})

export const localState = atomFamily({
  key: 'local/partitions',
  default: null
})

export const usePartitionListState = (partitionName) => {
  const [origin, setOrigin] = useRecoilState(originState({partitionName}));
  const [local, setLocal] = useRecoilState(localState({partitionName}));

  return {
    origin, setOrigin,
    local, setLocal,
  }
}
