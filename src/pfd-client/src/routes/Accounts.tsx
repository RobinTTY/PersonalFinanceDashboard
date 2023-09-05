import { useQuery } from "@apollo/client";
import { GetAccountsQuery } from "../queries/GetAccounts";
import { useDisclosure } from "@mantine/hooks";
import { Modal, Center, Loader, SimpleGrid, Button } from "@mantine/core";
import { AccountSummary } from "../components/account-summary/AccountSummary";
import { AccountSummaryProps } from "../components/account-summary/AccountSummaryProps";

import "./Accounts.css";

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
      <Modal opened={opened} onClose={close} title="Authentication" centered>
        test
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
