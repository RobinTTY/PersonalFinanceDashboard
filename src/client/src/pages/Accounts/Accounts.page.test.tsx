import { MockedProvider } from '@apollo/client/testing/react';
import { GetBankAccounts } from '@graphql-queries/GetBankAccounts';
import { GetBankAccountSyncStatus } from '@graphql-queries/GetBankAccountSyncStatus';
import { GetTransactionsByAccountId } from '@graphql-queries/GetTransactionsByAccountId';
import { SyncState } from '@graphql-types/graphql';
import { render, screen } from '@testing-library/react';
import { MantineProvider } from '@mantine/core';
import { createCache } from '@/graphql/cache';
import { AccountsPage } from './Accounts.page';

const ACCOUNT_ID = 'a1000000-0000-0000-0000-000000000001';

const neverSynced = {
  __typename: 'SyncStatus',
  state: SyncState.UpToDate,
  lastSuccessfulSync: null,
  lastError: null,
  nextRetry: null,
};

const mocks = [
  {
    request: { query: GetBankAccounts },
    result: {
      data: {
        bankAccounts: {
          __typename: 'BankAccountsConnection',
          nodes: [
            {
              __typename: 'BankAccount',
              id: ACCOUNT_ID,
              iban: null,
              name: 'Girokonto',
              balance: 100,
              currency: 'EUR',
              associatedInstitution: null,
              syncStatus: neverSynced,
            },
          ],
        },
      },
    },
  },
  {
    request: { query: GetTransactionsByAccountId, variables: { accountId: ACCOUNT_ID, first: 50 } },
    result: {
      data: {
        transactionsByAccountId: {
          __typename: 'TransactionsByAccountIdConnection',
          pageInfo: {
            __typename: 'PageInfo',
            hasNextPage: false,
            startCursor: null,
            endCursor: null,
          },
          edges: [],
        },
      },
    },
  },
  {
    // Loading the transactions synced them, so the account is now up to date
    request: { query: GetBankAccountSyncStatus, variables: { accountId: ACCOUNT_ID } },
    result: {
      data: {
        bankAccount: {
          __typename: 'BankAccount',
          id: ACCOUNT_ID,
          syncStatus: {
            ...neverSynced,
            lastSuccessfulSync: new Date(Date.now() - 5 * 60 * 1000).toISOString(),
          },
        },
      },
    },
  },
];

describe('AccountsPage', () => {
  it('updates the sync status once the transactions were loaded', async () => {
    render(
      <MockedProvider mocks={mocks} cache={createCache()}>
        <MantineProvider env="test">
          <AccountsPage />
        </MantineProvider>
      </MockedProvider>
    );

    expect(await screen.findByText('Not synced yet')).toBeInTheDocument();
    expect(await screen.findByText('Synced 5 minutes ago')).toBeInTheDocument();
  });
});
