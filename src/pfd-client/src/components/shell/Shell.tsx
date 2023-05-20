import { useState } from "react";
import {
  AppShell,
  Navbar,
  Header,
  Text,
  MediaQuery,
  Burger,
  useMantineTheme,
  Stack,
} from "@mantine/core";
import AppHeader from "../app-header/AppHeader";
import AccountButton from "../account-button/AccountButton";
import NavigationLink from "../navigation-link/NavigationLink";
import { IconArrowsExchange, IconHome } from "@tabler/icons-react";

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
          width={{ sm: 200, lg: 300 }}
        >
          <Navbar.Section grow>
            <Stack justify="flex-end" h={"100%"} spacing="xs" pb="md">
              <NavigationLink
                icon={<IconHome size="1rem" />}
                color={theme.colors.blue[6]}
                label="Dashboard"
              />
              <NavigationLink
                icon={<IconArrowsExchange size="1rem" />}
                color={theme.colors.teal[6]}
                label="Transactions"
              />
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
      <Text>Application Content</Text>
    </AppShell>
  );
};

export default Shell;
