import { useState } from "react";
import { Outlet, Link } from "react-router-dom";
import {
  AppShell,
  Navbar,
  Header,
  MediaQuery,
  Burger,
  useMantineTheme,
  Stack,
} from "@mantine/core";
import { IconArrowsExchange, IconHome, IconWallet } from "@tabler/icons-react";

import AppHeader from "../app-header/AppHeader";
import AccountButton from "../account-button/AccountButton";
import NavigationLink from "../navigation-link/NavigationLink";

// TODO: NavLinks aren't responsive yet
const Shell = () => {
  const theme = useMantineTheme();
  const [opened, setOpened] = useState(false);

  return (
    <AppShell
      styles={{
        main: {
          background:
            theme.colorScheme === "dark"
              ? theme.colors.dark[8]
              : theme.colors.gray[0],
        },
      }}
      navbarOffsetBreakpoint="sm"
      asideOffsetBreakpoint="sm"
      navbar={
        <Navbar
          p="md"
          hiddenBreakpoint="sm"
          hidden={!opened}
          width={{ sm: 250, lg: 250 }}
        >
          <Navbar.Section grow>
            <Stack justify="flex-end" h={"100%"} spacing="xs" pb="md">
              <Link to={`dashboard`}>
                <NavigationLink
                  icon={<IconHome size="1.25rem" />}
                  color="blue"
                  label="Dashboard"
                />
              </Link>
              <Link to={`accounts`}>
                <NavigationLink
                  icon={<IconWallet size="1.25rem" />}
                  color="violet"
                  label="Accounts"
                />
              </Link>
              <Link to={`transactions`}>
                <NavigationLink
                  icon={<IconArrowsExchange size="1.25rem" />}
                  color="teal"
                  label="Transactions"
                />
              </Link>
            </Stack>
          </Navbar.Section>
          <Navbar.Section>
            <AccountButton />
          </Navbar.Section>
        </Navbar>
      }
      header={
        <Header height={{ base: 50, md: 70 }} p="md">
          <div
            style={{ display: "flex", alignItems: "center", height: "100%" }}
          >
            <MediaQuery largerThan="sm" styles={{ display: "none" }}>
              <Burger
                opened={opened}
                onClick={() => setOpened((o) => !o)}
                size="sm"
                color={theme.colors.gray[6]}
                mr="xl"
              />
            </MediaQuery>
            <AppHeader />
          </div>
        </Header>
      }
    >
      <Outlet />
    </AppShell>
  );
};

export default Shell;
