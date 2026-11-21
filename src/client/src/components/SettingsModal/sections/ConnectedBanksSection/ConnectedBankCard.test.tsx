import {
  AuthenticationStatus,
  AuthRequestWithAccountsFragment,
  SyncState,
} from '@graphql-types/graphql';
import { render, screen, userEvent } from '@test-utils';
import { ConnectedBankCard } from './ConnectedBankCard';

const DAY = 24 * 60 * 60 * 1000;
const noop = () => {};

function createBank(
  overrides: Partial<AuthRequestWithAccountsFragment> = {}
): AuthRequestWithAccountsFragment {
  return {
    __typename: 'AuthenticationRequest',
    id: 'request-1',
    externalId: 'external-1',
    status: AuthenticationStatus.Active,
    authenticationLink: null,
    createdAt: new Date(Date.now() - 30 * DAY),
    accessExpiresAt: new Date(Date.now() + 60 * DAY),
    associatedAccounts: [
      {
        __typename: 'BankAccount',
        id: 'account-1',
        externalId: 'external-account-1',
        iban: 'DE89370400440532013000',
        name: 'Girokonto',
        description: null,
        accountType: null,
        balance: 100,
        currency: 'EUR',
        ownerName: null,
        includeInAnalytics: true,
        associatedInstitution: {
          __typename: 'BankingInstitution',
          id: 'institution-1',
          name: 'Test Bank',
          bic: 'TESTDEFFXXX',
          logoUri: null,
        },
        syncStatus: {
          __typename: 'SyncStatus',
          state: SyncState.Retrying,
          lastSuccessfulSync: null,
          lastError: 'Rate limit exceeded',
          nextRetry: new Date(Date.now() + 2 * 60 * 60 * 1000),
        },
      },
    ],
    ...overrides,
  };
}

function renderCard(bank: AuthRequestWithAccountsFragment, onReconnect = noop) {
  return render(
    <ConnectedBankCard
      bank={bank}
      onRequestDelete={noop}
      onReconnect={onReconnect}
      isDeleting={false}
    />
  );
}

function withSyncState(bank: AuthRequestWithAccountsFragment, state: SyncState) {
  return {
    ...bank,
    associatedAccounts: bank.associatedAccounts.map((account) => ({
      ...account,
      syncStatus: { ...account.syncStatus, state },
    })),
  };
}

describe('ConnectedBankCard', () => {
  it('shows when the access of an active connection expires', () => {
    renderCard(createBank());
    expect(screen.getByText('Access expires in 2 months')).toBeInTheDocument();
  });

  it('shows when the access of a connection expired', () => {
    const bank = createBank({
      status: AuthenticationStatus.Expired,
      accessExpiresAt: new Date(Date.now() - 3 * DAY),
    });
    renderCard(bank);
    expect(screen.getByText('Access expired 3 days ago')).toBeInTheDocument();
  });

  it('shows no expiry for connections that were never completed', () => {
    const bank = createBank({ status: AuthenticationStatus.Pending });
    renderCard(bank);
    expect(screen.queryByText(/Access expire/)).not.toBeInTheDocument();
  });

  it('shows the sync status of linked accounts', async () => {
    renderCard(createBank());
    await userEvent.click(screen.getByRole('button', { name: '1 linked account' }));
    expect(screen.getByText('Sync failed · retrying in 2 hours')).toBeInTheDocument();
  });

  it('offers to reconnect a bank whose accounts need it', async () => {
    const onReconnect = vi.fn();
    renderCard(withSyncState(createBank(), SyncState.NeedsReauthentication), onReconnect);
    await userEvent.click(screen.getByRole('button', { name: 'Reconnect' }));
    expect(onReconnect).toHaveBeenCalledWith({ id: 'institution-1', name: 'Test Bank' });
  });

  it('offers no reconnect while the accounts are synced', () => {
    renderCard(withSyncState(createBank(), SyncState.UpToDate));
    expect(screen.queryByRole('button', { name: 'Reconnect' })).not.toBeInTheDocument();
  });
});
