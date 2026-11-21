import { MockedProvider } from '@apollo/client/testing/react';
import { GetBankAccounts } from '@graphql-queries/GetBankAccounts';
import { GetTransactionsByAccountId } from '@graphql-queries/GetTransactionsByAccountId';
import { SyncState } from '@graphql-types/graphql';
import { render, screen } from '@testing-library/react';
import { MantineProvider } from '@mantine/core';
import { createCache } from '@/graphql/cache';
import { DataSynchronized } from '@/graphql/subscriptions/DataSynchronized';
import { AccountsPage } from '@/pages/Accounts/Accounts.page';
import { useRefetchOnDataSynchronized } from './useRefetchOnDataSynchronized';

const ACCOUNT_ID = 'a1000000-0000-0000-0000-000000000001';

const bankAccounts = (lastSuccessfulSync: string | null) => ({
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
            syncStatus: {
              __typename: 'SyncStatus',
              state: SyncState.UpToDate,
              lastSuccessfulSync,
              lastError: null,
              nextRetry: null,
            },
          },
        ],
      },
    },
  },
});

const mocks = [
  bankAccounts(null),
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
    maxUsageCount: 2,
  },
  {
    // Arrives after the page loaded
    request: { query: DataSynchronized },
    result: {
      data: {
        dataSynchronized: {
          __typename: 'DataSynchronization',
          completedAt: new Date().toISOString(),
        },
      },
    },
    delay: 200,
  },
  bankAccounts(new Date(Date.now() - 5 * 60 * 1000).toISOString()),
];

function SynchronizedAccountsPage() {
  useRefetchOnDataSynchronized();
  return <AccountsPage />;
}

describe('useRefetchOnDataSynchronized', () => {
  it('refetches the shown data when the server synchronized it', async () => {
    render(
      <MockedProvider mocks={mocks} cache={createCache()}>
        <MantineProvider env="test">
          <SynchronizedAccountsPage />
        </MantineProvider>
      </MockedProvider>
    );

    expect(await screen.findByText('Not synced yet')).toBeInTheDocument();
    expect(await screen.findByText('Synced 5 minutes ago')).toBeInTheDocument();
  });
});
