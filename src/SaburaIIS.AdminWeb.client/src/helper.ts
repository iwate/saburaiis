
export function errorMessage(err: unknown) {
  if (err instanceof Error) {
    return err.message;
  }
  switch (typeof err) {
    case 'string':
      return err;
    case 'object':
      return String((err as { message?: string }).message ?? err);
    default:
      return String(err);
  }
}
export function strings(...strs: (string | null | undefined)[]): string[] {
  return strs.filter(s => s) as string[];
}
// eslint-disable-next-line @typescript-eslint/no-explicit-any
export const patch = (current: any, keys: string[], value: unknown) => {
  const next = {...current};
  let _next = next;
  let _current = current;
  for (const key of keys.slice(0, keys.length - 1) ) {
    if (_next[key] instanceof Object) {
      _next[key] = {..._current[key]}
    }
    _next = _next[key];
    _current = _current[key];
  }
  _next[keys[keys.length - 1]] = value;
  return next;
}

export const sameValues = (a: unknown, b: unknown) => {
  if ((a === null || a === undefined) && (b === null || b === undefined))
    return true;
  if ((a === null || a === undefined) && !(b === null || b === undefined))
    return false;
  if (!(a === null || a === undefined) && (b === null || b === undefined))
    return false;

  if (a instanceof Array && b instanceof Array)
    return sameArrayValues(a, b);

  if (a instanceof Object && b instanceof Object)
    return sameObjectValues(a, b);

  return a === b;
}

const sameArrayValues = <T>(a: T[], b:T[]) => {
  if (a.length !== b.length)
    return false;
  
  for (let i = 0; i < a.length; i++) {
    if (!sameValues(a[i], b[i])) {
      return false;
    }
  }

  return true;
}

const sameObjectValues = <T extends object>(a: T, b: T) => {
  const aKeys = Object.keys(a) as (keyof T)[];
  const bKeys = Object.keys(b) as (keyof T)[];
  
  aKeys.sort();
  bKeys.sort();

  if (aKeys.length !== bKeys.length)
    return false;

  for (let i = 0; i < aKeys.length; i++) {
    const aKey = aKeys[i];
    const bKey = bKeys[i];

    if (aKey !== bKey)
      return false;

    if (!sameValues(a[aKey], b[bKey]))
      return false;
  }
  
  return true;
}

// eslint-disable-next-line @typescript-eslint/no-explicit-any
const jsonKeySort = (_: string, v: any) => (!(v instanceof Array || v === null) && typeof v == "object") ? Object.keys(v).sort().reduce((r: any, k:any) => { r[k] = v[k]; return r }, {}) : v;
export const serializeToJson = (obj: unknown) => JSON.stringify(obj, jsonKeySort, '\t');