// Fixture captured from the live GraphQL server and de-identified for the public
// repo: owner names, IBANs and authentication links are replaced with placeholders;
// balances, dates, amounts and public bank metadata are kept for realism. Regenerate
// by re-querying the backend and re-applying the same placeholder substitutions.

import type { GetBankingInstitutionsQuery } from '@graphql-types/graphql';

export const bankingInstitutionsData = {
  bankingInstitutions: {
    __typename: 'BankingInstitutionsConnection',
    pageInfo: {
      __typename: 'PageInfo',
      hasNextPage: true,
      hasPreviousPage: false,
      startCursor: 'MA==',
      endCursor: 'Nw==',
    },
    edges: [
      {
        __typename: 'BankingInstitutionsEdge',
        node: {
          __typename: 'BankingInstitution',
          id: '921eaee9-ead1-43b8-995f-f40418c083b7',
          thirdPartyId: 'DIREKT_HELADEF1822',
          name: '1822direkt',
          bic: 'HELADEF1822',
          logoUri: 'https://cdn-logos.gocardless.com/ais/DIREKT_HELADEF1822.png',
          countries: ['DE'],
        },
      },
      {
        __typename: 'BankingInstitutionsEdge',
        node: {
          __typename: 'BankingInstitution',
          id: 'ba3643ab-9f50-454e-9578-b57aecf0064a',
          thirdPartyId: 'ABNAMRO_FTSBDEFAXXX',
          name: 'ABN AMRO Bank Commercial',
          bic: 'FTSBDEFAXXX',
          logoUri:
            'https://storage.googleapis.com/gc-prd-institution_icons-production/UK/PNG/abnamrobank.png',
          countries: ['DE'],
        },
      },
      {
        __typename: 'BankingInstitutionsEdge',
        node: {
          __typename: 'BankingInstitution',
          id: 'c57484a2-de14-449c-9201-c31dcf67dc1e',
          thirdPartyId: 'AACHENER_BANK_GENODED1AAC',
          name: 'Aachener Bank',
          bic: 'GENODED1AAC',
          logoUri:
            'https://cdn-logos.gocardless.com/ais/VOLKSBANK_NIEDERGRAFSCHAFT_GENODEF1HOO.png',
          countries: ['DE'],
        },
      },
      {
        __typename: 'BankingInstitutionsEdge',
        node: {
          __typename: 'BankingInstitution',
          id: 'cdbd1978-c375-4775-9af3-fb960b02f2cf',
          thirdPartyId: 'AAREAL_AARBDE5W',
          name: 'Aareal Bank AG',
          bic: 'AARBDE5WXXX',
          logoUri: 'https://cdn-logos.gocardless.com/ais/AAREAL_AARBDE5W.png',
          countries: ['DE'],
        },
      },
      {
        __typename: 'BankingInstitutionsEdge',
        node: {
          __typename: 'BankingInstitution',
          id: '8b23f4a0-97f8-4158-8460-a8eec5d4add5',
          thirdPartyId: 'ABTSGMUNDER_BANK_GENODES1ABR',
          name: 'Abtsgmünder Bank',
          bic: 'GENODES1ABR',
          logoUri:
            'https://cdn-logos.gocardless.com/ais/VOLKSBANK_NIEDERGRAFSCHAFT_GENODEF1HOO.png',
          countries: ['DE'],
        },
      },
      {
        __typename: 'BankingInstitutionsEdge',
        node: {
          __typename: 'BankingInstitution',
          id: '156e300e-eb1c-4058-995a-3758626cc47c',
          thirdPartyId: 'AIRBUS_BANK_AGBMDEMMXXX',
          name: 'Airbus Bank',
          bic: 'AGBMDEMMXXX',
          logoUri: 'https://cdn-logos.gocardless.com/ais/AIRBUS_BANK_AGBMDEMMXXX.png',
          countries: ['DE'],
        },
      },
      {
        __typename: 'BankingInstitutionsEdge',
        node: {
          __typename: 'BankingInstitution',
          id: 'cbaf2186-a938-4ced-b293-f63e91a566e5',
          thirdPartyId: 'AIRWALLEX_EEA_AIPTAU32',
          name: 'Airwallex',
          bic: 'AIPTAU32',
          logoUri: 'https://cdn-logos.gocardless.com/ais/AIRWALLEX_AIPTAU32_1.png',
          countries: [
            'NO',
            'SE',
            'FI',
            'DK',
            'EE',
            'LV',
            'LT',
            'NL',
            'CZ',
            'ES',
            'PL',
            'BE',
            'DE',
            'AT',
            'BG',
            'HR',
            'CY',
            'FR',
            'GR',
            'HU',
            'IS',
            'IE',
            'IT',
            'LI',
            'LU',
            'MT',
            'PT',
            'RO',
            'SK',
            'SI',
          ],
        },
      },
      {
        __typename: 'BankingInstitutionsEdge',
        node: {
          __typename: 'BankingInstitution',
          id: '7188e75d-0b87-4a4a-92f5-d0fff6d525ee',
          thirdPartyId: 'AKTIVBANK_AKBADES1XXX',
          name: 'Aktivbank',
          bic: 'AKBADES1XXX',
          logoUri: 'https://cdn-logos.gocardless.com/ais/AKTIVBANK_AKBADES1XXX.png',
          countries: ['DE'],
        },
      },
    ],
  },
} satisfies GetBankingInstitutionsQuery;
