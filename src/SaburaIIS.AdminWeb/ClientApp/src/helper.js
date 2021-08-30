export const patch = (current, keys, value) => {
  const next = {...current};
  let _next = next;
  let _current = current;
  for (let key of keys.slice(0, keys.length - 1)) {
    if (_next[key].constructor === Object) {
      _next[key] = {..._current[key]}
    }
    _next = _next[key];
    _current = _current[key];
  }
  _next[keys[keys.length - 1]] = value;
  return next;
}

export const sameValues = (a, b) => {
  if ((a === null || a === undefined) && (b === null || b === undefined))
    return true;
  if ((a === null || a === undefined) && !(b === null || b === undefined))
    return false;
  if (!(a === null || a === undefined) && (b === null || b === undefined))
    return false;

  if (a.constructor === Array && b.constructor === Array)
    return sameArrayValues(a, b);

  if (a.constructor === Object && b.constructor === Object)
    return sameObjectValues(a, b);

  return a === b;
}

const sameArrayValues = (a, b) => {
  if (a.length !== b.length)
    return false;
  
  for (let i = 0; i < a.length; i++) {
    if (!sameValues(a[i], b[i]))
      return false;
  }

  return true;
}

const sameObjectValues = (a, b) => {
  const aKeys = Object.keys(a);
  const bKeys = Object.keys(b);
  
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

const jsonKeySort = (_, v) => (!(v instanceof Array || v === null) && typeof v == "object") ? Object.keys(v).sort().reduce((r, k) => { r[k] = v[k]; return r }, {}) : v;;;
export const serializeToJson = (obj) => JSON.stringify(obj, jsonKeySort, '\t');