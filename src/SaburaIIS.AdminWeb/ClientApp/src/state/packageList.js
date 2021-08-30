import { atom, useRecoilState, useRecoilValue } from "recoil";

const packagesSummaryState = atom({
  key: 'packagesSummary',
  default: {
    locationRoot: '%SystemDrive%\\inetpub\\sites',
    packages: [],
  }
})

export const usePackagesSummaryState = () => {
  return useRecoilState(packagesSummaryState);
}

export const usePackagesSummaryValue = () => {
  return useRecoilValue(packagesSummaryState);
}