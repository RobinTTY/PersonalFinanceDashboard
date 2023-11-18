import { Outlet } from 'react-router-dom';
import { AppShell, Box, Stack } from '@mantine/core';
import { IconArrowsExchange, IconHome, IconWallet } from '@tabler/icons-react';

import { AppHeader } from '@components/app-header/AppHeader';
import { AccountButton } from '@components/account-button/AccountButton';
import { NavigationLink } from '@components/navigation-link/NavigationLink';

import classes from './Shell.module.css';
import { AccountSummary } from '../account-summary/AccountSummary';

// TODO: NavLinks aren't responsive yet
export const Shell = () => (
  // TODO: Mobile layout
  <AppShell
    className={classes.shell}
    header={{ height: { base: 50, md: 70 } }}
    navbar={{ width: 230, breakpoint: 'sm' }}
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
        <Stack gap="md" justify="space-between" h="100%">
          <Stack gap="md">
            <AccountSummary
              description="Checkings"
              balance={1753.54}
              currency="USD"
              icon={<img src="/src/assets/img/testing/ing.png" alt="ING logo" />}
            />
          </Stack>
          <Stack justify="flex-end" gap="xs" pb="md">
            <NavigationLink
              icon={<IconHome size="1.25rem" />}
              color="blue"
              label="Dashboard"
              to="dashboard"
            />
            <NavigationLink
              icon={<IconWallet size="1.25rem" />}
              color="violet"
              label="Accounts"
              to="accounts"
            />
            <NavigationLink
              icon={<IconArrowsExchange size="1.25rem" />}
              color="teal"
              label="Transactions"
              to="transactions"
            />
          </Stack>
        </Stack>
      </AppShell.Section>
      <AppShell.Section>
        <Box className={classes['account-button-container']}>
          <AccountButton />
        </Box>
      </AppShell.Section>
    </AppShell.Navbar>
    <AppShell.Main className={classes['main-section']}>
      <Outlet />
    </AppShell.Main>
  </AppShell>
);
