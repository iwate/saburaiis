import { atom, useRecoilState, useRecoilValue } from "recoil";

const releasesState = atom({
  key: 'releases',
  default: []
})

export const useReleasesState = () => {
  return useRecoilState(releasesState);
}

export const useReleasesValue = () => {
  return useRecoilValue(releasesState);
}