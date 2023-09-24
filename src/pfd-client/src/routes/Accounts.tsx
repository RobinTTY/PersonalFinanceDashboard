import { useQuery } from '@apollo/client';
import { useDisclosure } from '@mantine/hooks';
import { Center, Loader, SimpleGrid, Button } from '@mantine/core';

import { BankAccount } from '@/graphql/types/graphql';
import { GetMinimalAccountsQuery } from '@/graphql/queries/GetAccounts';
import { AccountSummary } from '@/components/account-summary/AccountSummary';
import { NewAccountModal } from '@/modals/new-account-modal/NewAccountModal';

import classes from './Accounts.module.css';

// TODO: Add all accounts view?
// TODO: Add icon (e.g. bank logo)
export const Accounts = () => {
  const [addAccountModalOpen, { open: openAddAccountModal, close: closeAddAccountModal }] =
    useDisclosure(false);
  const { loading, data } = useQuery(GetMinimalAccountsQuery, {
    variables: {
      accountIds: [],
    },
  });

  const accounts: BankAccount[] = [];
  if (data && data.accounts && data.accounts.edges) {
    data.accounts.edges.forEach((account) => {
      accounts.push(account.node as BankAccount);
    });
  }

  // TODO: Create reusable loader component
  if (loading) {
    return (
      <Center h="100%">
        <Loader color="violet" />
      </Center>
    );
  }

  // TODO: common css class for 100% height/width container?
  return (
    <>
      <NewAccountModal opened={addAccountModalOpen} closeModal={closeAddAccountModal} />
      <div className={classes['accounts-container']}>
        <div>
          <SimpleGrid cols={{ base: 1, md: 2, xl: 4 }}>
            {accounts.map((account) => (
              // TODO: handle null
              <AccountSummary
                balance={account.balance}
                currency={account.currency!}
                description={account.description!}
                type={account.accountType!}
                key={account.id}
              />
            ))}
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
