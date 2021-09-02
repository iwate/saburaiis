import { Stack, StackItem, Nav} from '@fluentui/react'
import { useEffect } from 'react';
import { useHistory } from 'react-router-dom';
import { getPackageSummary, getPartitionNames } from '../api';
import { usePartitionListState, usePackagesSummaryState } from '../state';
import { BreadcrumbProvider, Breadcrumb } from './Breadcrumb';


const navStyles = {
  root: {
    width: 208,
    height: '100%',
    boxSizing: 'border-box',
    borderRight: '1px solid #eee',
    overflowY: 'auto',
  },
};

const breadcrumbToken = {
  padding:'0 16px 12px'
};

const breadcrumbStyle = {
  borderBottom: '1px solid #eee',
};

const Layout = ({ children }) => {
  const browserHistory = useHistory();
  const { local, setOrigin, setLocal } = usePartitionListState();
  const [packagesSummary, setPackagesSummary] = usePackagesSummaryState();

  useEffect(() => {
    (async () => {
      const names = await getPartitionNames();
      const partitions = names.map(name => ({name, hasDiff: false}));
      setOrigin(partitions)
      setLocal(partitions)
      const summary = await getPackageSummary();
      setPackagesSummary(summary);
    })()
  }, [setLocal, setOrigin, setPackagesSummary])
  
  if (local === null) {
    return <div>Loading...</div>
  }

  const partitions = [
    {
      key: 'partitions',
      name: 'Partitions',
      links: local.map(p => ({
        name: p.name + (p.hasDiff ? ' *' : ''),
        key: p.name,
        url: `/partitions/${p.name}`
      })).concat([
        {
          key: '+partition',
          name: 'New Partition',
          icon: 'Add',
          url: `/partitions/`
        }
      ])
    },
    {
      key: 'packages',
      name: 'Packages',
      links: packagesSummary.packages.map(name => ({
        key: name,
        name,
        url: `/packages/${name}`
      })).concat([
        {
          key: '+package',
          name: 'New Package',
          icon: 'Add',
          url: `/packages/`
        }
      ])
    }
  ];

  const onClickLink = (ev, item) => {
    ev.preventDefault(); 
    browserHistory.push(item.url);
  };

  return <BreadcrumbProvider>
    <Stack verticalFill>
      <Stack horizontal tokens={breadcrumbToken} style={breadcrumbStyle}>
        <StackItem grow>
          <Breadcrumb></Breadcrumb>
        </StackItem>
      </Stack>
      <Stack horizontal verticalFill grow style={{minHeight:0}}>
        <Nav styles={navStyles} groups={partitions} onLinkClick={onClickLink}></Nav>
        {children}
      </Stack>
    </Stack>
  </BreadcrumbProvider>
}

export default Layout;