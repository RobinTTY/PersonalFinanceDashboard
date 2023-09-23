import { useQuery } from '@apollo/client';
import { GetAccountsQuery } from '../graphql/queries/GetAccounts';
import { useDisclosure } from '@mantine/hooks';
import { Center, Loader, SimpleGrid, Button, Container } from '@mantine/core';

import { AccountSummary } from '../components/account-summary/AccountSummary';
import { AccountSummaryProps } from '../components/account-summary/AccountSummaryProps';
import { NewAccountModal } from '../modals/NewAccountModal';

import classes from './Accounts.module.css';

// TODO: Add all accounts view?
// TODO: Add icon (e.g. bank logo)
export const Accounts = () => {
  const [addAccountModalOpen, { open: openAddAccountModal, close: closeAddAccountModal }] =
    useDisclosure(false);
  const { loading, error, data } = useQuery(GetAccountsQuery, {
    variables: { first: 5 },
  });

  let accounts: any[] = [];
  if (data && data.accounts && data.accounts.edges)
    data.accounts.edges.map((account: any) => {
      accounts.push(account.node);
      // console.log(account.node);
    });

  if (loading)
    return (
      <Center h={'100%'}>
        <Loader color="violet" />
      </Center>
    );

  // TODO: common css class for 100% height/width container?
  return (
    <>
      <NewAccountModal
        opened={addAccountModalOpen}
        closeModal={closeAddAccountModal}
      ></NewAccountModal>
      <div className={classes['accounts-container']}>
        <div>
          <SimpleGrid cols={{ base: 1, md: 2, xl: 4 }}>
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
