import useSWR from "swr";
import { getCertificates } from "../api";
import { IDropdownOption } from "@fluentui/react";

const formatter = new Intl.DateTimeFormat('default', { year: 'numeric', month: 'numeric', day: 'numeric' });
function format(datetimeStr?: string) {
  return datetimeStr ? formatter.format(new Date(datetimeStr)) : ''
}

export type ICertificateOptionsActions = {
  refresh: () => void
}

export default function useCertificateOptions(): [state: IDropdownOption[] | undefined, error: string|null, actions: ICertificateOptionsActions] {
  const {data, error, mutate} = useSWR('certificates', getCertificates);
  const refresh = () => {
    mutate();
  }
  if (!data || error) {
    return [undefined, error, { refresh }];
  }

  const options = (data??[]).map(c => ({ 
    key: c.thumbprint, 
    text: `${c.name}(${c.thumbprint}) ${format(c.notBefore)} ~ ${format(c.expiresOn)}`
  }));
  
  return [[{ key: '', text: '' }, ...options], null, { refresh }]
}