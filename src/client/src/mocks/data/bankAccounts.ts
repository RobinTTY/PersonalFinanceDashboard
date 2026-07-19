// Fixture captured from the live GraphQL server and de-identified for the public
// repo: owner names, IBANs and authentication links are replaced with placeholders;
// balances, dates, amounts and public bank metadata are kept for realism. Regenerate
// by re-querying the backend and re-applying the same placeholder substitutions.

import type { GetBankAccountsQuery } from '@graphql-types/graphql';

export const bankAccountsData = {
  bankAccounts: {
    __typename: 'BankAccountsConnection',
    nodes: [
      {
        __typename: 'BankAccount',
        id: '56ea92c0-c578-41fd-ba27-836dfc90ae06',
        iban: 'DE89370400440532013000',
        name: 'C24 Gemeinschaftskonto',
        balance: 92.3,
        currency: 'EUR',
        associatedInstitution: {
          __typename: 'BankingInstitution',
          name: 'C24',
          logoUri: 'https://cdn-logos.gocardless.com/ais/C24_DEFFDEFF.png',
        },
      },
      {
        __typename: 'BankAccount',
        id: '588946a8-da08-471b-b944-4890a2ce6281',
        iban: null,
        name: 'C24 PocketZINS Smart',
        balance: 0.0,
        currency: 'EUR',
        associatedInstitution: {
          __typename: 'BankingInstitution',
          name: 'C24',
          logoUri: 'https://cdn-logos.gocardless.com/ais/C24_DEFFDEFF.png',
        },
      },
      {
        __typename: 'BankAccount',
        id: '097c2685-71b1-48a3-8161-937f2ddec43f',
        iban: 'DE89370400440532013001',
        name: 'C24 Smartkonto',
        balance: 346.64,
        currency: 'EUR',
        associatedInstitution: {
          __typename: 'BankingInstitution',
          name: 'C24',
          logoUri: 'https://cdn-logos.gocardless.com/ais/C24_DEFFDEFF.png',
        },
      },
      {
        __typename: 'BankAccount',
        id: 'b45b24fc-2a58-46b9-887d-a3a5820ed30e',
        iban: 'DE89370400440532013002',
        name: 'Girokonto',
        balance: 0.0,
        currency: 'EUR',
        associatedInstitution: {
          __typename: 'BankingInstitution',
          name: 'Santander',
          logoUri: 'https://cdn-logos.gocardless.com/ais/SANTANDER_PL_CORP_WBKPPLPP.png',
        },
      },
    ],
  },
} satisfies GetBankAccountsQuery;
