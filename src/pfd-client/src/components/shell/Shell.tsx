import { Outlet, Link } from 'react-router-dom';
import { AppShell, Stack } from '@mantine/core';
import { IconArrowsExchange, IconHome, IconWallet } from '@tabler/icons-react';

import { AppHeader } from '../app-header/AppHeader';
import { AccountButton } from '../account-button/AccountButton';
import { NavigationLink } from '../navigation-link/NavigationLink';

import classes from './Shell.module.css';

// TODO: NavLinks aren't responsive yet
export const Shell = () => (
  // TODO: Mobile Burger menu
  <AppShell
    className={classes.shell}
    header={{ height: { base: 50, md: 70 } }}
    navbar={{ width: 250, breakpoint: 'sm' }}
    padding="md"
  >
    <AppShell.Header p="md">
      <div style={{ display: 'flex', alignItems: 'center', height: '100%' }}>
        {/* <MediaQuery largerThan="sm" styles={{ display: 'none' }}>
            <Burger
              opened={opened}
              onClick={() => setOpened((o) => !o)}
              size="sm"
              color={theme.colors.gray[6]}
              mr="xl"
            />
          </MediaQuery> */}
        <AppHeader />
      </div>
    </AppShell.Header>
    <AppShell.Navbar p="md">
      <AppShell.Section grow>
        <Stack justify="flex-end" h="100%" gap="xs" pb="md">
          {/* TODO: These links should probably be implemented via polymorphic components */}
          <Link to="dashboard">
            <NavigationLink icon={<IconHome size="1.25rem" />} color="blue" label="Dashboard" />
          </Link>
          <Link to="accounts">
            <NavigationLink icon={<IconWallet size="1.25rem" />} color="violet" label="Accounts" />
          </Link>
          <Link to="transactions">
            <NavigationLink
              icon={<IconArrowsExchange size="1.25rem" />}
              color="teal"
              label="Transactions"
            />
          </Link>
        </Stack>
      </AppShell.Section>
      <AppShell.Section>
        <AccountButton />
      </AppShell.Section>
    </AppShell.Navbar>
    <AppShell.Main className={classes['main-section']}>
      <Outlet />
    </AppShell.Main>
  </AppShell>
);
