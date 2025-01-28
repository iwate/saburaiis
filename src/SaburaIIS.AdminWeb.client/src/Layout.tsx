import { Outlet, useNavigate } from "react-router";
import { Breadcrumbs, BreadcrumbsProvider } from "./assemblies/Breadcrumbs";
import { INavLinkGroup, INavProps, Nav, Stack, StackItem } from "@fluentui/react";
import usePartitionListState from "./hooks/usePartitionListState";
import usePackagesSummaryState from "./hooks/usePackagesSummaryState";
import { WorkingContextProvider } from "./components/WorkingContext";

const breadcrumbToken = {
  padding: '0 16px 12px'
};

const breadcrumbStyle = {
  borderBottom: '1px solid #eee',
};

const navStyles = {
  root: {
    width: 208,
    height: '100%',
    boxSizing: 'border-box',
    borderRight: '1px solid #eee',
    overflowY: 'auto',
  },
};

function Layout() {
  const [origin] = usePartitionListState();
  const [packagesSummary] = usePackagesSummaryState();
  const packages = packagesSummary?.packages ?? [];
  const partitions: INavLinkGroup[] = [
    {
      //key: 'partitions',
      name: 'Partitions',
      links: (origin??[]).map(name => ({
        name: name,// + (p.hasDiff ? ' *' : ''),
        key: name,
        icon: '',
        url: `/partitions/${name}`,
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
      //key: 'packages',
      name: 'Packages',
      links: packages.map(name => ({
        key: name,
        name,
        icon: '',
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

  const navigate = useNavigate();
  const onClickLink: INavProps['onLinkClick'] = (ev, item) => {
    ev?.preventDefault();
    if (item?.url) {
      navigate(item.url);
    }
  };

  return (
    <>
      <WorkingContextProvider>
        <BreadcrumbsProvider>
          <Stack verticalFill>
            <Stack horizontal tokens={breadcrumbToken} style={breadcrumbStyle}>
              <StackItem grow>
                <Breadcrumbs />
              </StackItem>
            </Stack>
            <Stack horizontal verticalFill grow style={{ minHeight: 0 }}>
              <Nav styles={navStyles} groups={partitions} onLinkClick={onClickLink}></Nav>
              <Outlet />
            </Stack>
          </Stack>
        </BreadcrumbsProvider>
      </WorkingContextProvider>
    </>
  )
}

export default Layout
