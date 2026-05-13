import { axe, render } from '@test-utils';
import { fireEvent, screen } from '@testing-library/react';
import { AuthRequestWithAccountsFragment } from '@graphql-types/graphql';
import { AccountsHeader } from './AccountsHeader';

type Account = AuthRequestWithAccountsFragment['associatedAccounts'][number];

const noop = () => {};

const c24Account: Account = {
  __typename: 'BankAccount',
  id: '1',
  thirdPartyId: 'acc_001',
  iban: 'DE89370400440532013000',
  name: 'C24 Gemeinschaftskonto',
  description: null,
  accountType: 'CACC',
  balance: 468.15,
  currency: 'EUR',
  ownerName: 'Robin Mueller',
  associatedInstitution: {
    __typename: 'BankingInstitution',
    name: 'C24 Bank',
    bic: 'DEFFDEFFXXX',
    logoUri: null,
  },
};

const sparkasseAccount: Account = {
  __typename: 'BankAccount',
  id: '2',
  thirdPartyId: 'acc_002',
  iban: 'DE91100000000123456789',
  name: 'Sparkasse Girokonto',
  description: null,
  accountType: 'CACC',
  balance: 1234.56,
  currency: 'EUR',
  ownerName: 'Robin Mueller',
  associatedInstitution: {
    __typename: 'BankingInstitution',
    name: 'Sparkasse',
    bic: 'SSKMDEMMXXX',
    logoUri: null,
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
    expect(screen.getByText('468.15 EUR')).toBeInTheDocument();
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
        loadingAccounts={true}
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
    fireEvent.click(screen.getByRole('menuitem', { name: 'Sparkasse Girokonto' }));
    expect(onAccountChange).toHaveBeenCalledWith('2');
  });
});
