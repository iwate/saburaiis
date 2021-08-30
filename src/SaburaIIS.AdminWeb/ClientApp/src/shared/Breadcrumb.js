import { useContext, createContext, useState, useEffect } from "react";
import { Breadcrumb as FluentBreadcrumb } from '@fluentui/react'
import { useHistory, useLocation, useRouteMatch } from "react-router";

const breadcrumbDefaultValue = [
  { text: 'SaburaIIS', key: '/', href: '/' }
];

export const BreadcrumbContext = createContext([]);

export const useBreadcrumb = (items) => {
  const context = useContext(BreadcrumbContext);
  const location = useLocation();
  const { url } = useRouteMatch();
  useEffect(() => {
    if (url === location.pathname) {
      context.setBreadcrumb(items)
    }
  }, [location]);
  return [context.breadcrumb, context.setBreadcrumb];
};

const equals = (array1, array2, key) => {
  if (array1.length !== array2.length)
    return false;

  for (let i = 0; i < array1.length; i++)
    if (array1[i][key] !== array2[i][key])
      return false;

  return true;
}
export const BreadcrumbProvider = ({ children }) => {
  const [breadcrumb, _setBreadcrumb] = useState([]);
  const setBreadcrumb = (items) => {
    if (Array.isArray(items)) {
      const newValue = breadcrumbDefaultValue.concat(items);
      if (!equals(breadcrumb, newValue, 'key')) {
        _setBreadcrumb(newValue);
      }
    }
  };

  return <BreadcrumbContext.Provider value={{ breadcrumb, setBreadcrumb }}>
    {children}
  </BreadcrumbContext.Provider>
}

export const Breadcrumb = () => {
  const browserHistory = useHistory();
  const onClickLink = (ev, item) => {
    ev.preventDefault();
    browserHistory.push(item.href);
  };
  const [breadcrumb] = useBreadcrumb();
  return <FluentBreadcrumb
    items={breadcrumb.map(item => ({ ...item, onClick: onClickLink }))}
    maxDisplayedItems={10}
    ariaLabel="Breadcrumb with items rendered as buttons"
    overflowAriaLabel="More links">
  </FluentBreadcrumb>
}