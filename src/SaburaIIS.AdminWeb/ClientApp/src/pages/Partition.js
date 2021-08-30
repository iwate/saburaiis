import { Stack, Nav, CommandBar, Dialog, DialogFooter, DefaultButton, PrimaryButton } from '@fluentui/react'
import { useBoolean } from '@fluentui/react-hooks'
import { Route, Switch, useHistory, useParams, useRouteMatch } from 'react-router-dom';
import { useBreadcrumb } from '../shared/Breadcrumb';
import { ApplicationPool } from './ApplicationPool';
import { usePartitionListState, usePartitionState } from '../state/index';
import { useEffect } from 'react';
import { getPartition, removePartition, replacePartition } from '../api';
import { sameValues, serializeToJson } from '../helper';
import { MonacoDiffEditor } from 'react-monaco-editor';
import { Site } from './Site';
import { ScaleSets } from './ScaleSets';
import { ApplicationPoolCreate } from './ApplicationPoolCreate';
import { SiteCreate } from './SiteCreate';
import { Instances } from './Instances';

const navStyles = {
  root: {
    width: 208,
    height: '100%',
    boxSizing: 'border-box',
    borderRight: '1px solid #eee',
    overflowY: 'auto',
  },
};
const Partition = () => {
  const browserHistory = useHistory();
  const { path, url } = useRouteMatch();
  const { partitionName } = useParams();
  const { setLocal: setListLocal } = usePartitionListState();
  const { origin, local, setOrigin, setLocal } = usePartitionState(partitionName);
  const [ hiddenDeleteDialog, { toggle: toggleHiddenDeleteDialog }] = useBoolean(true);
  const hasDiff = !sameValues(origin, local);
  
  useBreadcrumb([
    { text: partitionName, key: `/partitions/${partitionName}` },
  ]);

  useEffect(() => {
    (async () => {
      if (origin === null) {
        const partition = await getPartition(partitionName);
        setOrigin(partition);
        setLocal(partition);
      }
    })()
  }, [partitionName])

  useEffect(() => {
    setListLocal(list => {
      const index = list.findIndex(item => item.name === partitionName);
      return [...list.slice(0, index), {name:partitionName, hasDiff}, ...list.slice(index+1)];
    })
  }, [origin, local]);

  if (local === null) {
    return <div>Loading ...</div>
  }

  const items = [
    {
      name: 'Application Pools',
      links: local.applicationPools.map(pool => ({
        name: pool.name,
        url: `${url}/apppools/${pool.name}`,
        
      })).concat([
        {
          key: '+apppool',
          name: 'New AppPool',
          icon: 'Add',
          url: `${url}/apppools/`
        }
      ])
    },
    {
      name: 'Sites',
      links: local.sites.map(site => ({
        name: site.name,
        url: `${url}/sites/${site.name}`
      })).concat([
        {
          key: '+site',
          name: 'New Site',
          icon: 'Add',
          url: `${url}/sites/`
        }
      ])
    },
    {
      name: 'Scale Sets',
      links: local.scaleSets.map(sset => ({
        name: sset.name
      })).concat([
        {
          key: '+scaleset',
          name: 'Manage Scale Set',
          icon: 'Add',
          url: `${url}/scalesets/`
        }
      ])
    },
    {
      links:[
        {
          name: 'Instances',
          url: `${url}/instances/`,
          isExpanded: true,
        }
      ]
    }
  ];

  const onClickLink = (ev, item) => {
    ev.preventDefault(); 
    browserHistory.push(item.url);
  };

  const commands = [
    {
      key: 'delete',
      text: 'Delete Partition',
      iconProps: { iconName: 'Delete' },
      onClick: toggleHiddenDeleteDialog,
      split: true,
    },
  ];

  if (hasDiff) {
    commands.push(
      {
        key: 'apply',
        text: 'Apply',
        iconProps: { iconName: 'Checkmark' },
        onClick: async () => {
          await replacePartition(local);
          setOrigin(local);
        },
        split: true,
      },
      {
        key: 'discard',
        text: 'Discard this changes',
        iconProps: { iconName: 'Undo' },
        onClick: () => { setLocal(origin) },
      }
    )
  }

  const deletePartition = async () => {
    await removePartition(origin);
    setListLocal(list => {
      const index = list.findIndex(item => item.name === partitionName);
      return [...list.slice(0,index),...list.slice(index+1)];
    })
    browserHistory.push('/');
  }

  return <>
    <Stack horizontal verticalFill style={{width:'100%'}}>
      <Nav groups={items} onLinkClick={onClickLink} styles={navStyles}></Nav>
      <Switch>
          <Route exact path={path}>
            <Stack style={{width: '100%'}}>
              <CommandBar items={commands} style={{borderBottom: '1px solid #eee', paddingBottom: 4}}/>
              <MonacoDiffEditor
                language="json"
                original={serializeToJson(origin)}
                value={serializeToJson(local)}
                options={{readOnly: true}}/>
            </Stack>
          </Route>
          <Route path={`${path}/apppools/:apppoolName`} component={ApplicationPool}></Route>
          <Route path={`${path}/apppools/`} component={ApplicationPoolCreate}></Route>
          <Route path={`${path}/sites/:siteName`} component={Site}></Route>
          <Route path={`${path}/sites/`} component={SiteCreate}></Route>
          <Route path={`${path}/scalesets/`} component={ScaleSets}></Route>
          <Route path={`${path}/instances/`} component={Instances}></Route>
      </Switch>
    </Stack>
    <Dialog
      hidden={hiddenDeleteDialog}
      subText="You cannot undo the deletion. Are you sure you want to delete this partition?"
      title="CAUTION"
      minWidth={348}
      onDismiss={toggleHiddenDeleteDialog}
    >
      <DialogFooter>
        <DefaultButton
          onClick={deletePartition}
        >Yes, I delete this partition</DefaultButton>
        <PrimaryButton onClick={toggleHiddenDeleteDialog}>Cancel</PrimaryButton>
      </DialogFooter>
    </Dialog>
  </>
}

export default Partition;