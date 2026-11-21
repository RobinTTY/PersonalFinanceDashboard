import { BankAccountSummaryFragment, SyncState } from '@graphql-types/graphql';
import { ComponentPreview } from '../storybook/ComponentPreview';
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

export default { title: 'Accounts/AccountsHeader' };

export function SingleAccount() {
  return (
    <ComponentPreview canvas={{ center: false }} withSpacing>
      <AccountsHeader
        currentAccount={c24Account}
        accounts={[c24Account]}
        loadingAccounts={false}
        onAccountChange={noop}
        onAddAccount={noop}
      />
    </ComponentPreview>
  );
}

export function MultipleAccounts() {
  return (
    <ComponentPreview canvas={{ center: false }} withSpacing>
      <AccountsHeader
        currentAccount={c24Account}
        accounts={[c24Account, sparkasseAccount]}
        loadingAccounts={false}
        onAccountChange={noop}
        onAddAccount={noop}
      />
    </ComponentPreview>
  );
}

export function SyncFailed() {
  return (
    <ComponentPreview canvas={{ center: false }} withSpacing>
      <AccountsHeader
        currentAccount={sparkasseAccount}
        accounts={[c24Account, sparkasseAccount]}
        loadingAccounts={false}
        onAccountChange={noop}
        onAddAccount={noop}
      />
    </ComponentPreview>
  );
}

export function NoAccount() {
  return (
    <ComponentPreview canvas={{ center: false }} withSpacing>
      <AccountsHeader
        currentAccount={null}
        accounts={[]}
        loadingAccounts={false}
        onAccountChange={noop}
        onAddAccount={noop}
      />
    </ComponentPreview>
  );
}

export function Loading() {
  return (
    <ComponentPreview canvas={{ center: false }} withSpacing>
      <AccountsHeader
        currentAccount={null}
        accounts={[]}
        loadingAccounts
        onAccountChange={noop}
        onAddAccount={noop}
      />
    </ComponentPreview>
  );
}
