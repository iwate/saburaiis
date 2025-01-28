import { Stack, Nav, INavProps, INavLink, INavLinkGroup } from '@fluentui/react'
import { useParams, useNavigate } from 'react-router';
import { useContext, useEffect, useMemo } from 'react';
import { WorkingContext } from '../components/WorkingContext';
import { Outlet } from 'react-router';
import usePartitionState from '../hooks/usePartitionState';

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
  partitionName: string
}

export default function Partition() {
  const navigate = useNavigate();
  const { partitionName } = useParams<RouteParams>();
  const { value: context, actions: { setPartition } } = useContext(WorkingContext);
  const [origin] = usePartitionState(partitionName);
  const local = useMemo(() => context.partitions[partitionName!], [context.partitions, partitionName]);
  
  useEffect(() => {
    if (origin && !local) {
      setPartition({ ...origin }, false);
    }
  }, [origin, local]); // eslint-disable-line react-hooks/exhaustive-deps

  const items = useMemo<INavLinkGroup[]>(() => {
    const url = `/partitions/${partitionName}`;
    const local = context.partitions[partitionName!];
    if (!local) {
      return [];
    }
    return [
      {
        name: 'Application Pools',
        links: local.applicationPools.map(pool => ({
          name: pool.name,
          url: `${url}/apppools/${pool.name}`,

        }) as INavLink).concat([
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
        }) as INavLink).concat([
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
        }) as INavLink).concat([
          {
            key: '+scaleset',
            name: 'Manage Scale Set',
            icon: 'Add',
            url: `${url}/scalesets/`
          }
        ])
      },
      {
        links: [
          {
            name: 'Instances',
            url: `${url}/instances/`,
            isExpanded: true,
          }
        ]
      }
    ];
  }, [context.partitions]); // eslint-disable-line react-hooks/exhaustive-deps

  const onClickLink: INavProps['onLinkClick'] = (ev, item) => {
    ev?.preventDefault();
    if (item?.url) {
      navigate(item.url);
    }
  };
  
  return <>
    <Stack horizontal verticalFill style={{ width: '100%' }}>
      <Nav groups={items} onLinkClick={onClickLink} styles={navStyles}></Nav>
      <Outlet/>
    </Stack>
  </>
}
