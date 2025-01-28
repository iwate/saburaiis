import { INavLinkGroup, INavProps, Nav, Stack } from "@fluentui/react";
import { Outlet, useParams } from "react-router";
import { useBreadcrumbs } from "../assemblies/Breadcrumbs";
import useReleaseListState from "../hooks/useReleaseListState";
import { useNavigate } from "react-router";
import { useMemo } from "react";

const stackStyles = {
  width: '100%',
  height: '100%'
}

const navStyles = {
  root: {
    width: 208,
    height: '100%',
    boxSizing: 'border-box',
    borderRight: '1px solid #eee',
    overflowY: 'auto',
  },
};

type RouteParams = {
  packageName: string
}

export default function Package() {
  const { packageName } = useParams<RouteParams>();
  const navigate = useNavigate();
  useBreadcrumbs([
    { text: 'Packages', key: `/packages/` },
    { text: packageName!, key: `/packages/${packageName}` },
  ]);
  const [releases] = useReleaseListState(packageName!);

  const items: INavLinkGroup[] = useMemo(() => [{
    links: [
      {
        key: 'new',
        name: 'New Release',
        icon: 'Add',
        url: `/packages/${packageName}/releases/`
      }
    ].concat(releases?.map(release => ({
      key: release,
      name: release,
      icon: '',
      url: `/packages/${packageName}/releases/${release}`
    })) ?? [])
  }], [packageName, releases]);

  const onLinkClick: INavProps['onLinkClick'] = (ev, item) => {
    ev?.preventDefault();
    if (item) {
      navigate(item.url);
    }
  };

  return <Stack horizontal verticalFill style={stackStyles}>
    <Nav groups={items} onLinkClick={onLinkClick} styles={navStyles}></Nav>
    <Outlet/>
  </Stack>
}