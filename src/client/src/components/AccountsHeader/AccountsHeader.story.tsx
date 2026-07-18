import { AuthRequestWithAccountsFragment } from '@graphql-types/graphql';
import { ComponentPreview } from '../storybook/ComponentPreview';
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
  includeInAnalytics: true,
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
  includeInAnalytics: true,
  associatedInstitution: {
    __typename: 'BankingInstitution',
    name: 'Sparkasse',
    bic: 'SSKMDEMMXXX',
    logoUri: null,
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
