import { useContext, createContext, useState, FC, PropsWithChildren, Dispatch, SetStateAction, useEffect, useMemo, useCallback } from "react";
import { Breadcrumb as FluentBreadcrumb, IBreadcrumbItem } from '@fluentui/react'
import { useNavigate } from "react-router";

const breadcrumbDefaultValue: IBreadcrumbItem[] = [
  { text: 'SaburaIIS', key: '/', href: '/' }
];

export type IBreadcrumbContext = {
  value: IBreadcrumbItem[],
  set: Dispatch<SetStateAction<IBreadcrumbItem[]>>
}

export const BreadcrumbContext = createContext<IBreadcrumbContext>({ value: [], set: () => { } });

export function useBreadcrumbs(items?: IBreadcrumbItem[]): [value: IBreadcrumbItem[], set: Dispatch<SetStateAction<IBreadcrumbItem[]>>] {
  const { value, set } = useContext(BreadcrumbContext);
  useEffect(() => {
    if (items) {
      set(breadcrumbDefaultValue.concat(items));
    }
  }, [])
  //const location = useLocation();
  //const { url } = useRouteMatch();
  //useEffect(() => {
  //  if (url === location.pathname) {
  //    context.setBreadcrumb(items)
  //  }
  //}, [location, context, url, items]);
  return [value, set];
};

//const equals = (array1:, array2, key) => {
//  if (array1.length !== array2.length)
//    return false;

//  for (let i = 0; i < array1.length; i++)
//    if (array1[i][key] !== array2[i][key])
//      return false;

//  return true;
//}
export const BreadcrumbsProvider: FC<PropsWithChildren> = ({ children }) => {
  const [value, set] = useState<IBreadcrumbItem[]>(breadcrumbDefaultValue);
  //const setBreadcrumb = (items) => {
  //  if (Array.isArray(items)) {
  //    const newValue = breadcrumbDefaultValue.concat(items);
  //    if (!equals(breadcrumb, newValue, 'key')) {
  //      _setBreadcrumb(newValue);
  //    }
  //  }
  //};

  return <BreadcrumbContext.Provider value={{value, set}}>
    {children}
  </BreadcrumbContext.Provider>
}

export const Breadcrumbs = () => {
  const navigate = useNavigate();
  const onClick = useCallback<Required<IBreadcrumbItem>['onClick']>((ev, item) => {
    if (item?.href) {
      ev?.preventDefault();
      navigate(item.href);
    }
  }, [navigate])
  const [value] = useBreadcrumbs();
  const items: IBreadcrumbItem[] = useMemo(() => value.map(item => ({ ...item, onClick })), [value, onClick]);
  return <FluentBreadcrumb
    items={items}
    maxDisplayedItems={10}
    ariaLabel="Breadcrumb with items rendered as buttons"
    overflowAriaLabel="More links">
  </FluentBreadcrumb>
}