import { useQuery } from "@apollo/client";
import { GetAccountsQuery } from "../queries/GetAccounts";
import { useDisclosure } from "@mantine/hooks";
import {
  UnstyledButton,
  Modal,
  Center,
  Loader,
  SimpleGrid,
  Button,
  Group,
  Text,
} from "@mantine/core";
import { IconCoins, IconGraph } from "@tabler/icons-react";

import { AccountSummary } from "../components/account-summary/AccountSummary";
import { AccountSummaryProps } from "../components/account-summary/AccountSummaryProps";

import "./Accounts.css";

/**
 * The available options when adding an account.
 * @param param0 The button icon and text.
 * @returns A button with the icon and text.
 */
const ModalButton = ({ icon, description, action }: AccountOptionsProps) => {
  return (
    <UnstyledButton
      onClick={action}
      sx={(theme) => ({
        display: "block",
        width: "100%",
        padding: theme.spacing.xl,
        borderRadius: theme.radius.sm,
        color:
          theme.colorScheme === "dark" ? theme.colors.dark[0] : theme.black,
        backgroundColor:
          theme.colorScheme === "dark" ? theme.colors.dark[6] : theme.white,
        "&:hover": {
          backgroundColor:
            theme.colorScheme === "dark"
              ? theme.colors.dark[4]
              : theme.colors.gray[0],
        },
      })}
    >
      <Center>{icon}</Center>
      <Center>
        <Text fz="lg" fw={700}>
          {description}
        </Text>
      </Center>
    </UnstyledButton>
  );
};

interface AccountOptionsProps {
  icon: JSX.Element;
  description: string;
  action: () => void;
}

// TODO: Add all accounts view?
// TODO: Add icon (e.g. bank logo)
export const Accounts = () => {
  const [opened, { open, close }] = useDisclosure(false);
  const { loading, error, data } = useQuery(GetAccountsQuery, {
    variables: { first: 5 },
  });

  let accounts: any[] = [];
  if (data)
    data.accounts.edges.map((account: any) => {
      accounts.push(account.node);
      // console.log(account.node);
    });

  if (loading)
    return (
      <Center h={"100%"}>
        <Loader color="violet" />
      </Center>
    );

  // TODO: common css class for 100% height/width container?
  return (
    <>
      <Modal
        opened={opened}
        onClose={close}
        title={
          <Text fz="lg" fw={700} pr="xl">
            What type of account would you like to add?
          </Text>
        }
        centered
        size="auto"
      >
        <Group className="account-type-selection" p="xs">
          {/* TODO: Create reusable component */}
          <ModalButton
            icon={<IconCoins size="4rem" />}
            description="Savings Account"
            action={() => console.log("Savings Account")}
          />
          <ModalButton
            icon={<IconGraph size="4rem" />}
            description="Investment Account"
            action={() => console.log("Investment Account")}
          />
        </Group>
      </Modal>
      <div className="accounts-container">
        <div>
          <SimpleGrid
            mt="2%"
            cols={2}
            breakpoints={[{ maxWidth: "xs", cols: 1 }]}
          >
            {accounts.map((account: AccountSummaryProps) => {
              // TODO: Add key
              return <AccountSummary {...account} key={account.balance} />;
            })}
          </SimpleGrid>
        </div>
        <Center py="md">
          <Button onClick={open} size="md">
            Add Account
          </Button>
        </Center>
      </div>
    </>
  );
};
