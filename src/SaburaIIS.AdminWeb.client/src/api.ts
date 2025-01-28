import { ICertificate, IPackage, IPackagesSummary, IPartition, IRelease, ISnapshot, IVirtualMachine } from "./type";

export const getPartitionNames = async (): Promise<string[]> => {
  const response = await fetch(`/api/partitions`);
  if (!response.ok)
    throw new Error("fetch error");
  return await response.json(); 
}

export const getPartition = async (partitionName: string): Promise<IPartition> => {
  const response = await fetch(`/api/partitions/${partitionName}`);
  if (!response.ok)
    throw new Error("fetch error");
  const data = await response.json();
  data['@etag'] = response.headers.get('etag');
  return data;
}

export const replacePartition = async (partition: IPartition): Promise<IPartition['@etag']> => {
  const response = await fetch(`/api/partitions/${partition.name}`, {
    method: 'put',
    headers: { 'Content-Type': 'application/json', 'If-Match': partition['@etag'] ?? '' },
    body: JSON.stringify(partition)
  });
  if (!response.ok)
    throw new Error("fetch error");
  return response.headers.get('etag');
}

export const addPartition = async (name: string) => {
  const response = await fetch(`/api/partitions/`, {
    method: 'post',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({name})
  });
  if (!response.ok)
    throw (await getErrorMessage(response));
}

export const removePartition = async (partition: IPartition) => {
  const response = await fetch(`/api/partitions/${partition.name}`, {
    method: 'delete',
    headers: { 'If-Match': partition['@etag'] ?? '' }
  });
  if (!response.ok)
    throw (await getErrorMessage(response));
}

export const getInstances = async (partitionName: string, partitionEtag: string): Promise<IVirtualMachine[]> => {
  const response = await fetch(`/api/partitions/${partitionName}/instances?filter=${partitionEtag}`);
  if (!response.ok)
    throw new Error("fetch error");
  return await response.json();
}

export const getInstance = async (partitionName: string, scaleSetName: string, instanceName: string): Promise<IVirtualMachine> => {
  const response = await fetch(`/api/partitions/${partitionName}/instances/${scaleSetName}/${instanceName}`);
  if (!response.ok)
    throw new Error("fetch error");
  return await response.json();
}

export const getSnapshots = async (partitionName: string, scaleSetName: string, instanceName: string): Promise<string[]> => {
  const response = await fetch(`/api/partitions/${partitionName}/instances/${scaleSetName}/${instanceName}/snapshots`);
  if (!response.ok)
    throw new Error("fetch error");
  return await response.json();
}

export const getSnapshot = async (partitionName: string, scaleSetName: string, instanceName: string, timestamp: string): Promise<ISnapshot> => {
  const response = await fetch(`/api/partitions/${partitionName}/instances/${scaleSetName}/${instanceName}/snapshots/${encodeURIComponent(timestamp)}`);
  if (!response.ok)
    throw new Error("fetch error");
  return await response.json();
}

export const getPackageSummary = async (): Promise<IPackagesSummary> => {
  const response = await fetch(`/api/packages/`);
  if (!response.ok)
    throw new Error("fetch error");
  return await response.json();
}

export const getPackage = async (name: string): Promise<IPackage> => {
  const response = await fetch(`/api/packages/${name}`);
  if (!response.ok)
    throw new Error("fetch error");
  const data = await response.json();
  data['@etag'] = response.headers.get('etag');
  return data;
}

export const removePackage = async (packageName: string, etag: string = '*') => {
  const response = await fetch(`/api/packages/${packageName}`, {
    method: 'delete',
    headers: { 'If-Match': etag }
  });
  if (!response.ok)
    throw (await getErrorMessage(response));
}

export const getReleaseVersions = async (packageName: string): Promise<string[]> => {
  const response = await fetch(`/api/packages/${packageName}/releases`);
  if (!response.ok)
    throw new Error("fetch error");
  return await response.json();
}

export const addPackage = async (name: string) => {
  const response = await fetch(`/api/packages/`, {
    method: 'post',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ name })
  });
  if (!response.ok)
    throw (await getErrorMessage(response));
}

export const addRelease = async (packageName: string, version: string, url: string) => {
  const response = await fetch(`/api/packages/${packageName}/releases`, {
    method: 'post',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ version, url })
  });
  if (!response.ok)
    throw (await getErrorMessage(response));
}

export const getRelease = async (packageName: string, versionName: string): Promise<IRelease> => {
  const response = await fetch(`/api/packages/${packageName}/releases/${versionName}`);
  if (!response.ok)
    throw new Error("fetch error");
  return await response.json();
}

export const getCertificates = async (): Promise<ICertificate[]> => {
  const response = await fetch(`/api/certificates`);
  if (!response.ok)
    throw new Error("fetch error");
  return await response.json();
}

const getErrorMessage = async (response: Response) => {
  if (response.headers.has('Content-Type')
   && response.headers.get('Content-Type')!.match(/application\/[^\s]*json/)) {
    const data = await response.json();
    if (data.errors) 
      return Object.values(data.errors).join('\n');
    if (data.message)
      return data.message;
    if (data.title)
      return data.title
    return JSON.stringify(data);
  }
  else {
    return await response.text();
  }
}
