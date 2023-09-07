import { useState } from "react";
import { useQuery } from "@apollo/client";
import { GetAccountsQuery } from "../queries/GetAccounts";
import { useDisclosure, useCounter } from "@mantine/hooks";
import {
  Modal,
  Center,
  Loader,
  SimpleGrid,
  Button,
  Group,
  Text,
  Grid,
  Container,
} from "@mantine/core";
import { IconCoins, IconGraph } from "@tabler/icons-react";
import { US } from "country-flag-icons/react/3x2";
import { CA } from "country-flag-icons/react/3x2";
import { GB } from "country-flag-icons/react/3x2";
import { AU } from "country-flag-icons/react/3x2";
import { NZ } from "country-flag-icons/react/3x2";

import { AccountSummary } from "../components/account-summary/AccountSummary";
import { AccountSummaryProps } from "../components/account-summary/AccountSummaryProps";
import { ModalButton } from "../components/modal-button/ModalButton";

import "./Accounts.css";
import { SearchBox } from "../components/search-box/SearchBox";

type AccountType = "savings" | "investment";

const AccountTypeSelectionStep = ({
  selectionAction,
}: AccountTypeSelectionStepProps) => {
  return (
    <Group className="account-type-selection" p="xs">
      {/* TODO: Create reusable component */}
      <ModalButton
        icon={<IconCoins size="4rem" />}
        description="Savings Account"
        action={() => selectionAction("savings")}
      />
      <ModalButton
        icon={<IconGraph size="4rem" />}
        description="Investment Account"
        action={() => selectionAction("investment")}
      />
    </Group>
  );
};

interface AccountTypeSelectionStepProps {
  selectionAction: (accountType: AccountType) => void;
}

const SavingsAccountSetupStep = () => {
  const countries = [
    { name: "United States", icon: <US /> },
    { name: "Canada", icon: <CA /> },
    { name: "United Kingdom", icon: <GB /> },
    { name: "Australia", icon: <AU /> },
    { name: "New Zealand", icon: <NZ /> },
  ];
  const [searchFilter, setSearchFilter] = useState("");

  const onFilterUpdate = (filter: string) => {
    setSearchFilter(filter);
  };

  return (
    <Container p={0} size="sm">
      <SearchBox
        pl="xs"
        pr="xs"
        placeholder="Filter countries..."
        actionIconActive={false}
        value={searchFilter}
        onChange={(event) => onFilterUpdate(event.currentTarget.value)}
      />
      <Grid pt="md" pb="md" pl="md" pr="md">
        {countries.map((country) => {
          return (
            <Grid.Col span={6} key={country.name}>
              <ModalButton
                icon={country.icon}
                iconHeight="32px"
                iconPosition="left"
                description={country.name}
                includeChevron={true}
                padding="md"
                action={() => {}}
              />
            </Grid.Col>
          );
        })}
      </Grid>
    </Container>
  );
};

// TODO: Add all accounts view?
// TODO: Add icon (e.g. bank logo)
export const Accounts = () => {
  const [
    addAccountModalOpen,
    { open: openAddAccountModal, close: closeAddAccountModal },
  ] = useDisclosure(false);
  const [accountType, setAccountType] = useState<AccountType>();
  const [addAccountStep, addAccountStepHandler] = useCounter(1, {
    min: 1,
    max: 4,
  });
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

  const onNewAccount = (accountType: AccountType) => {
    setAccountType(accountType);
    addAccountStepHandler.increment();
  };

  const getActiveNewAccountStep = () => {
    switch (addAccountStep) {
      case 1:
        return <AccountTypeSelectionStep selectionAction={onNewAccount} />;
      case 2:
        switch (accountType) {
          case "savings":
            return <SavingsAccountSetupStep />;
          case "investment":
            return <Text>Step 2</Text>;
          default:
            throw new Error("Invalid account type");
        }
      default:
        return <Text>Step {addAccountStep}</Text>;
    }
  };

  const newAccountModal = (
    <Modal
      opened={addAccountModalOpen}
      onClose={() => {
        closeAddAccountModal();
        addAccountStepHandler.reset();
      }}
      title={
        <Text fz="lg" fw={700} pl=".4rem">
          {addAccountStep === 1 &&
            "What type of account would you like to add?"}
          {addAccountStep === 2 && "Select your country"}
        </Text>
      }
      centered
      size="auto"
    >
      {getActiveNewAccountStep()}
    </Modal>
  );

  // TODO: common css class for 100% height/width container?
  return (
    <>
      {newAccountModal}
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
          <Button onClick={openAddAccountModal} size="md">
            Add Account
          </Button>
        </Center>
      </div>
    </>
  );
};
