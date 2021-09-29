import { DetailsList, Stack, Link, CommandBar, SelectionMode } from "@fluentui/react";
import { useCallback, useEffect, useState } from "react";
import { Route, Switch, useHistory, useParams, useRouteMatch } from "react-router";
import { getInstances } from "../api";
import { objectStateOptions } from "../constants";
import { useBreadcrumb } from "../shared/Breadcrumb";
import { usePartitionState } from "../state";
import { History } from "./History";

export const Instances = () => {
  const browserHistory = useHistory();
  const { path, url } = useRouteMatch();
  const { partitionName } = useParams();
  const { origin } = usePartitionState(partitionName);
  const [instances, setInstances] = useState([]);
  useBreadcrumb([
    { text: partitionName, key: `/partitions/${partitionName}`, href: `/partitions/${partitionName}` },
    { text: 'Instances' },
  ]);

  const loadInstances = useCallback(async () => {
    const instances = await getInstances(partitionName, origin['@etag']);
    setInstances(instances.reduce((a,b) => {
      return a.concat(b.current.sites.reduce((c,d) => {
        return c.concat(d.applications.reduce((e,f) => {
          const apppool = b.current.applicationPools.find(pool => pool.name === f.applicationPoolName);
          return e.concat(f.virtualDirectories.map(vdir => ({
            scaleSetName: b.scaleSetName,
            instanceName: b.name,
            siteName: d.name,
            siteState: objectStateOptions.find(item => item.key === d.state).text,
            appPath: f.path,
            vdirPath: vdir.path,
            vdirPhysicalPath: vdir.physicalPath,
            apppoolName: apppool.name,
            apppoolState: objectStateOptions.find(item => item.key === apppool.state).text,
          })))
        }, []))
      }, []))
    }, []));
  }, [setInstances, partitionName])

  useEffect(() => {
    loadInstances();
  }, [partitionName, loadInstances])

  const columns = [
    {
      key: 'scaleSetName',
      name: 'Scale Set',
      fieldName: 'scaleSetName',
      isResizable: true,
      minWidth: 32, maxWidth: 64,
    },
    {
      key: 'instanceName',
      name: 'Instance',
      fieldName: 'instanceName',
      flexGrow: 1,
      isResizable: true,
      minWidth: 32, maxWidth: 64,
      onRender: item => (
        <Link
          key={`${item.scaleSetName}-${item.instanceName}`}
          onClick={() => browserHistory.push(`${url}${item.scaleSetName}/${item.instanceName}`)}>
          {item.instanceName}
        </Link>
      )
    },
    {
      key: 'siteName',
      name: 'Site',
      fieldName: 'siteName',
      flexGrow: 1,
      isResizable: true,
      minWidth: 64, maxWidth: 128,
    },
    {
      key: 'apppoolName',
      name: 'Application Pool',
      fieldName: 'apppoolName',
      flexGrow: 1,
      isResizable: true,
      minWidth: 64, maxWidth: 128,
    },
    {
      key: 'appPath',
      name: 'Application',
      fieldName: 'appPath',
      flexGrow: 1,
      isResizable: true,
      minWidth: 64, maxWidth: 96,
    },
    {
      key: 'vdirPath',
      name: 'Virtual Directory',
      fieldName: 'vdirPath',
      flexGrow: 1,
      isResizable: true,
      minWidth: 64, maxWidth: 128,
    },
    {
      key: 'vdirPhysicalPath',
      name: 'Path',
      fieldName: 'vdirPhysicalPath',
      flexGrow: 1,
      isResizable: true,
      minWidth: 200, maxWidth: 300,
    },
    {
      key: 'siteState',
      name: 'Site State',
      fieldName: 'siteState',
      flexGrow: 1,
      isResizable: true,
      minWidth: 32, maxWidth: 64,
    },
    {
      key: 'apppoolState',
      name: 'Application Pool State',
      fieldName: 'apppoolState',
      flexGrow: 1,
      isResizable: true,
      minWidth: 64, maxWidth: 128,
    }
  ];

  const commands = [
    {
      key: 'reload',
      text: 'Reload',
      iconProps: { iconName: 'Refresh' },
      onClick: loadInstances,
    }
  ]
  return <Stack style={{ width: '100%' }}>
    <Switch>
      <Route exact path={path}>
        <Stack grow style={{ width: '100%', overflowY: 'auto' }}>
          <CommandBar items={commands} style={{ borderBottom: '1px solid #eee', paddingBottom: 4 }} />
          <DetailsList
            items={instances}
            columns={columns}
            selectionMode={SelectionMode.none}
          />
        </Stack>
      </Route>
      <Route path={`${path}:scaleSetName/:instanceName`} component={History} />
    </Switch>
  </Stack>
}