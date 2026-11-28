import { BankAccountSummaryFragment, SyncState } from '@graphql-types/graphql';
import { axe, render } from '@test-utils';
import { fireEvent, screen } from '@testing-library/react';
import { formatMoney } from '@utility';
import { AccountsHeader } from './AccountsHeader';

type Account = BankAccountSummaryFragment;

const noop = () => {};

const c24Account: Account = {
  __typename: 'BankAccount',
  id: '1',
  iban: 'DE89370400440532013000',
  name: 'C24 Gemeinschaftskonto',
  balance: 468.15,
  currency: 'EUR',
  associatedInstitution: {
    __typename: 'BankingInstitution',
    name: 'C24 Bank',
    logoUri: null,
  },
  syncStatus: {
    __typename: 'SyncStatus',
    state: SyncState.UpToDate,
    lastSuccessfulSync: new Date(Date.now() - 2 * 60 * 60 * 1000),
    lastError: null,
    nextRetry: null,
  },
};

const sparkasseAccount: Account = {
  __typename: 'BankAccount',
  id: '2',
  iban: 'DE91100000000123456789',
  name: 'Sparkasse Girokonto',
  balance: 1234.56,
  currency: 'EUR',
  associatedInstitution: {
    __typename: 'BankingInstitution',
    name: 'Sparkasse',
    logoUri: null,
  },
  syncStatus: {
    __typename: 'SyncStatus',
    state: SyncState.Retrying,
    lastSuccessfulSync: new Date(Date.now() - 26 * 60 * 60 * 1000),
    lastError: 'Rate limit exceeded. Too many requests',
    nextRetry: new Date(Date.now() + 2 * 60 * 60 * 1000),
  },
};

describe('AccountsHeader', () => {
  axe([
    <AccountsHeader
      key="1"
      currentAccount={c24Account}
      accounts={[c24Account]}
      loadingAccounts={false}
      onAccountChange={noop}
      onAddAccount={noop}
    />,
  ]);

  it('renders the current balance', () => {
    render(
      <AccountsHeader
        currentAccount={c24Account}
        accounts={[c24Account]}
        loadingAccounts={false}
        onAccountChange={noop}
        onAddAccount={noop}
      />
    );
    expect(screen.getByText(formatMoney(468.15, 'EUR'))).toBeInTheDocument();
  });

  it('renders the account name', () => {
    render(
      <AccountsHeader
        currentAccount={c24Account}
        accounts={[c24Account]}
        loadingAccounts={false}
        onAccountChange={noop}
        onAddAccount={noop}
      />
    );
    expect(screen.getByText('C24 Gemeinschaftskonto')).toBeInTheDocument();
  });

  it('renders institution initials when no logo is available', () => {
    render(
      <AccountsHeader
        currentAccount={c24Account}
        accounts={[c24Account]}
        loadingAccounts={false}
        onAccountChange={noop}
        onAddAccount={noop}
      />
    );
    expect(screen.getByText('CB')).toBeInTheDocument();
  });

  it('shows Add account option in the dropdown', () => {
    render(
      <AccountsHeader
        currentAccount={c24Account}
        accounts={[c24Account]}
        loadingAccounts={false}
        onAccountChange={noop}
        onAddAccount={noop}
      />
    );
    fireEvent.click(screen.getByText('C24 Gemeinschaftskonto'));
    expect(screen.getByRole('menuitem', { name: /add account/i })).toBeInTheDocument();
  });

  it('calls onAddAccount when Add account is clicked', () => {
    const onAddAccount = vi.fn();
    render(
      <AccountsHeader
        currentAccount={c24Account}
        accounts={[c24Account]}
        loadingAccounts={false}
        onAccountChange={noop}
        onAddAccount={onAddAccount}
      />
    );
    fireEvent.click(screen.getByText('C24 Gemeinschaftskonto'));
    fireEvent.click(screen.getByRole('menuitem', { name: /add account/i }));
    expect(onAddAccount).toHaveBeenCalledTimes(1);
  });

  it('shows — when no account is selected', () => {
    render(
      <AccountsHeader
        currentAccount={null}
        accounts={[]}
        loadingAccounts={false}
        onAccountChange={noop}
        onAddAccount={noop}
      />
    );
    expect(screen.getByText('—')).toBeInTheDocument();
  });

  it('shows Loading… when accounts are loading', () => {
    render(
      <AccountsHeader
        currentAccount={null}
        accounts={[]}
        loadingAccounts
        onAccountChange={noop}
        onAddAccount={noop}
      />
    );
    expect(screen.getByText('Loading…')).toBeInTheDocument();
  });

  it('does not show other accounts in the menu with a single account', () => {
    render(
      <AccountsHeader
        currentAccount={c24Account}
        accounts={[c24Account]}
        loadingAccounts={false}
        onAccountChange={noop}
        onAddAccount={noop}
      />
    );
    fireEvent.click(screen.getByText('C24 Gemeinschaftskonto'));
    expect(screen.queryByRole('menuitem', { name: 'Sparkasse Girokonto' })).not.toBeInTheDocument();
  });

  it('opens account switcher and calls onAccountChange when a menu item is clicked', () => {
    const onAccountChange = vi.fn();
    render(
      <AccountsHeader
        currentAccount={c24Account}
        accounts={[c24Account, sparkasseAccount]}
        loadingAccounts={false}
        onAccountChange={onAccountChange}
        onAddAccount={noop}
      />
    );
    fireEvent.click(screen.getByText('C24 Gemeinschaftskonto'));
    fireEvent.click(screen.getByRole('menuitem', { name: /Sparkasse Girokonto/ }));
    expect(onAccountChange).toHaveBeenCalledWith('2');
  });

  it('shows the sync status of the current account', () => {
    render(
      <AccountsHeader
        currentAccount={c24Account}
        accounts={[c24Account]}
        loadingAccounts={false}
        onAccountChange={noop}
        onAddAccount={noop}
      />
    );
    expect(screen.getByText('Synced 2 hours ago')).toBeInTheDocument();
  });

  it('marks accounts whose sync failed in the menu', () => {
    render(
      <AccountsHeader
        currentAccount={c24Account}
        accounts={[c24Account, sparkasseAccount]}
        loadingAccounts={false}
        onAccountChange={noop}
        onAddAccount={noop}
      />
    );
    fireEvent.click(screen.getByText('C24 Gemeinschaftskonto'));
    expect(
      screen.getByRole('menuitem', { name: 'Sparkasse Girokonto Sync failed' })
    ).toBeInTheDocument();
    expect(screen.getByRole('menuitem', { name: 'C24 Gemeinschaftskonto' })).toBeInTheDocument();
  });
});
