// Fixture captured from the live GraphQL server and de-identified for the public
// repo: owner names, IBANs and authentication links are replaced with placeholders;
// balances, dates, amounts and public bank metadata are kept for realism. Regenerate
// by re-querying the backend and re-applying the same placeholder substitutions.

import type { GetBankAccountQuery } from '@graphql-types/graphql';

type BankAccountDetail = NonNullable<GetBankAccountQuery['bankAccount']>;

export const bankAccountDetailsById: Record<string, BankAccountDetail> = {
  '37a3289e-5949-4870-a13a-f6e7759307b9': {
    __typename: 'BankAccount',
    id: '37a3289e-5949-4870-a13a-f6e7759307b9',
    iban: 'DE63600697100987654321',
    name: 'Checkings',
    description: null,
    accountType: 'Current',
    balance: 1913.12,
    currency: 'EUR',
    ownerName: 'Jane Doe',
    includeInAnalytics: false,
    associatedInstitution: {
      __typename: 'BankingInstitution',
      name: 'Sandbox Finance',
      bic: 'GENODES1RGF',
      logoUri: 'https://cdn-logos.gocardless.com/ais/SANDBOXFINANCE_SFIN0000.png',
    },
  },
  'b97e1883-b035-42bc-8c20-f5164535aa92': {
    __typename: 'BankAccount',
    id: 'b97e1883-b035-42bc-8c20-f5164535aa92',
    iban: 'DE43600697101234567890',
    name: 'Checkings',
    description: null,
    accountType: 'Current',
    balance: 1913.12,
    currency: 'EUR',
    ownerName: 'John Doe',
    includeInAnalytics: false,
    associatedInstitution: {
      __typename: 'BankingInstitution',
      name: 'Sandbox Finance',
      bic: 'GENODES1RGF',
      logoUri: 'https://cdn-logos.gocardless.com/ais/SANDBOXFINANCE_SFIN0000.png',
    },
  },
  '097c2685-71b1-48a3-8161-937f2ddec43f': {
    __typename: 'BankAccount',
    id: '097c2685-71b1-48a3-8161-937f2ddec43f',
    iban: 'DE89370400440532013001',
    name: 'C24 Smartkonto',
    description: 'C24 Smartkonto',
    accountType: 'Current',
    balance: 346.64,
    currency: 'EUR',
    ownerName: 'Adam Silver',
    includeInAnalytics: true,
    associatedInstitution: {
      __typename: 'BankingInstitution',
      name: 'C24',
      bic: 'DEFFDEFF',
      logoUri: 'https://cdn-logos.gocardless.com/ais/C24_DEFFDEFF.png',
    },
  },
  '56ea92c0-c578-41fd-ba27-836dfc90ae06': {
    __typename: 'BankAccount',
    id: '56ea92c0-c578-41fd-ba27-836dfc90ae06',
    iban: 'DE89370400440532013000',
    name: 'C24 Gemeinschaftskonto',
    description: 'C24 Gemeinschaftskonto',
    accountType: 'Current',
    balance: 92.3,
    currency: 'EUR',
    ownerName: 'Adam and Kate Silver',
    includeInAnalytics: true,
    associatedInstitution: {
      __typename: 'BankingInstitution',
      name: 'C24',
      bic: 'DEFFDEFF',
      logoUri: 'https://cdn-logos.gocardless.com/ais/C24_DEFFDEFF.png',
    },
  },
  '588946a8-da08-471b-b944-4890a2ce6281': {
    __typename: 'BankAccount',
    id: '588946a8-da08-471b-b944-4890a2ce6281',
    iban: null,
    name: 'C24 PocketZINS Smart',
    description: null,
    accountType: 'Savings',
    balance: 0.0,
    currency: 'EUR',
    ownerName: 'Adam Silver',
    includeInAnalytics: true,
    associatedInstitution: {
      __typename: 'BankingInstitution',
      name: 'C24',
      bic: 'DEFFDEFF',
      logoUri: 'https://cdn-logos.gocardless.com/ais/C24_DEFFDEFF.png',
    },
  },
  'b45b24fc-2a58-46b9-887d-a3a5820ed30e': {
    __typename: 'BankAccount',
    id: 'b45b24fc-2a58-46b9-887d-a3a5820ed30e',
    iban: 'DE89370400440532013002',
    name: 'Girokonto',
    description: null,
    accountType: 'Current',
    balance: 0.0,
    currency: 'EUR',
    ownerName: 'Adam Silver',
    includeInAnalytics: true,
    associatedInstitution: {
      __typename: 'BankingInstitution',
      name: 'Santander',
      bic: 'SCFBDE33XXX',
      logoUri: 'https://cdn-logos.gocardless.com/ais/SANTANDER_PL_CORP_WBKPPLPP.png',
    },
  },
} as unknown as Record<string, BankAccountDetail>;
