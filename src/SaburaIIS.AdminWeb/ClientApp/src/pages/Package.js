import { CommandBar, Nav, Stack } from "@fluentui/react";
import { useEffect, useState } from "react";
import { Route, Switch, useHistory, useParams, useRouteMatch } from "react-router";
import { getReleaseVersions } from "../api";
import { useBreadcrumb } from "../shared/Breadcrumb";
import { useReleasesState } from "../state";
import { Release } from "./Release";
import { ReleaseCreate } from "./ReleaseCreate";

const navStyles = {
  root: {
    width: 208,
    height: '100%',
    boxSizing: 'border-box',
    borderRight: '1px solid #eee',
    overflowY: 'auto',
  },
};

export const Package = () => {
  const browserHistory = useHistory();
  const { path, url } = useRouteMatch();
  const { packageName } = useParams();
  const [releases, setReleases] = useReleasesState();

  useEffect(() => {
    (async () => {
      const versions = await getReleaseVersions(packageName);
      setReleases(versions);
    })()
  }, [packageName]);

  useBreadcrumb([
    { text: 'Packages', key: `/packages/` },
    { text: packageName, key: `/packages/${packageName}` },
  ]);

  const items = [{
    links: [
      {
        key: 'new',
        name: 'New Release',
        icon: 'Add',
        url: `${url}/releases/`
      }
    ].concat(releases.map(release => ({
      key: release,
      name: release,
      url: `${url}/releases/${release}`
    })))
  }]

  const commands = [
    {
      key: 'new',
      text: 'New Release',
      iconProps: { iconName: 'Add' },
      onClick: () => { },
    },
  ];

  const onClickLink = (ev, item) => {
    ev.preventDefault();
    browserHistory.push(item.url);
  };

  return <Stack horizontal verticalFill style={{ width: '100%' }}>
    <Nav groups={items} onLinkClick={onClickLink} styles={navStyles}></Nav>
    <Switch>
      <Route path={`${path}/releases/:versionName`} component={Release}></Route>
      <Route path={`${path}/releases/`} component={ReleaseCreate}></Route>
    </Switch>
  </Stack>
}