import { atomFamily, useRecoilState } from "recoil";

export const originState = atomFamily({
  key: 'origin/partition',
  default: null
})

export const localState = atomFamily({
  key: 'local/partition',
  default: null
})

export const usePartitionState = (partitionName) => {
  const [origin, setOrigin] = useRecoilState(originState({partitionName}));
  const [local, setLocal] = useRecoilState(localState({partitionName}));

  const removeApplicationPool = name => {
    const index = local.applicationPools.findIndex(pool => pool.name === name);
    setLocal({
      ...local,
      applicationPools: [
        ...local.applicationPools.slice(0, index), ...local.applicationPools.slice(index+1)
      ]
    })
  }
  const removeSite = name => {
    const index = local.sites.findIndex(site => site.name === name);
    setLocal({
      ...local,
      sites: [
        ...local.sites.slice(0, index), ...local.sites.slice(index+1)
      ]
    })
  }

  return {
    origin, setOrigin,
    local, setLocal,
    removeApplicationPool,
    removeSite
  }
}