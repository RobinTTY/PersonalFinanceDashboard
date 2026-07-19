// Fixture captured from the live GraphQL server and de-identified for the public
// repo: owner names, IBANs and authentication links are replaced with placeholders;
// balances, dates, amounts and public bank metadata are kept for realism. Regenerate
// by re-querying the backend and re-applying the same placeholder substitutions.

import type { GetAuthRequestsWithAccountsQuery } from '@graphql-types/graphql';

type AuthenticationRequest = GetAuthRequestsWithAccountsQuery['authenticationRequests'][number];

export const authenticationRequestsData: AuthenticationRequest[] = [
  {
    __typename: 'AuthenticationRequest',
    id: '236df904-b02f-4c7a-a2ea-6c5254a682fc',
    thirdPartyId: 'e91a776d-ed66-4775-ba99-fc62462152bb',
    status: 'ACTIVE',
    authenticationLink:
      'https://ob.gocardless.com/ob-psd2/start/00000000-0000-0000-0000-000000000000/SANDBOXFINANCE_GENODES1RGF',
    createdAt: '2026-05-02T21:39:39.958Z',
    associatedAccounts: [
      {
        __typename: 'BankAccount',
        id: '37a3289e-5949-4870-a13a-f6e7759307b9',
        thirdPartyId: 'f2f5ca20-1397-4cbf-93df-7f7348e4ab95',
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
      {
        __typename: 'BankAccount',
        id: 'b97e1883-b035-42bc-8c20-f5164535aa92',
        thirdPartyId: '81226c35-4c96-4ce2-8b3a-dbc983c0407a',
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
    ],
  },
  {
    __typename: 'AuthenticationRequest',
    id: 'd6ddd9f1-3900-4dc1-9dfd-243887839443',
    thirdPartyId: '6341e179-0f9a-48cf-a8aa-6c03d54e7c5c',
    status: 'ACTIVE',
    authenticationLink:
      'https://ob.gocardless.com/ob-psd2/start/00000000-0000-0000-0000-000000000000/C24_DEFFDEFF',
    createdAt: '2026-05-02T22:34:19.324Z',
    associatedAccounts: [
      {
        __typename: 'BankAccount',
        id: '097c2685-71b1-48a3-8161-937f2ddec43f',
        thirdPartyId: '0139ea21-2a9a-469f-82a3-4b80bca14a24',
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
      {
        __typename: 'BankAccount',
        id: '56ea92c0-c578-41fd-ba27-836dfc90ae06',
        thirdPartyId: '7bf4b81e-3354-4edd-bcfc-d39e1ce29f2b',
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
      {
        __typename: 'BankAccount',
        id: '588946a8-da08-471b-b944-4890a2ce6281',
        thirdPartyId: 'ee97a055-dd14-4e04-8000-8ee4ac5420de',
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
    ],
  },
  {
    __typename: 'AuthenticationRequest',
    id: 'f484d494-f82f-41b0-b51a-2af853da8c05',
    thirdPartyId: 'a436d552-d370-4cde-af6d-93b62f9d39e1',
    status: 'ACTIVE',
    authenticationLink:
      'https://ob.gocardless.com/ob-psd2/start/00000000-0000-0000-0000-000000000000/SANTANDER_DE_SCFBDE33',
    createdAt: '2026-07-12T00:16:31.018Z',
    associatedAccounts: [
      {
        __typename: 'BankAccount',
        id: 'b45b24fc-2a58-46b9-887d-a3a5820ed30e',
        thirdPartyId: 'caa4e9dd-dead-42ae-9fb6-c9ed955d4d97',
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
    ],
  },
] as unknown as AuthenticationRequest[];
