import { DEFAULT_APPLICATION_POOL, DEFAULT_SITE } from "./constants"

export type IApplicationPool = typeof DEFAULT_APPLICATION_POOL & { recycleRequestAt?: string }

export type ISite = typeof DEFAULT_SITE

export type IScaleSet = {
  name: string
}

export type IPartition = {
  '@etag': string | null
  name: string
  applicationPools: IApplicationPool[]
  sites: ISite[]
  scaleSets: IScaleSet[]
}

export type IPackagesSummary = {
  locationRoot: string
  packages: string[]
}

export type IRelease = {
  version: string
  url: string
  releaseAt: string
}

export type ICertificate = {
  name: string
  version: string
  thumbprint: string
  notBefore?: string
  expiresOn?: string
}

export type ISnapshot = {
  scaleSetName: string
  name: string
  timestamp: string
  applicationPools: IApplicationPool[]
  sites: ISite[]
}

export type IVirtualMachine = {
  scaleSetName: string
  name: string
  partitionETag: string
  current: ISnapshot
}

export type IPackage = {
  '@etag': string | null
  name: string
}