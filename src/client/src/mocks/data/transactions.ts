// Fixture captured from the live GraphQL server and de-identified for the public
// repo: owner names, IBANs and authentication links are replaced with placeholders;
// balances, dates, amounts and public bank metadata are kept for realism. Regenerate
// by re-querying the backend and re-applying the same placeholder substitutions.

import type { GetTransactionsByAccountIdQuery } from '@graphql-types/graphql';

export const transactionsFirstPage = {
  transactionsByAccountId: {
    __typename: 'TransactionsByAccountIdConnection',
    pageInfo: {
      __typename: 'PageInfo',
      hasNextPage: false,
      startCursor: 'MA==',
      endCursor: 'MTk=',
    },
    edges: [
      {
        __typename: 'TransactionsByAccountIdEdge',
        node: {
          __typename: 'Transaction',
          id: 'ac274270-6880-4234-8cbc-d9860b577b4d',
          valueDate: '2026-07-18T00:00:00.000Z',
          amount: -64.95,
          payer: 'KAUFLAND KOELN KALK 22',
          payee: 'Adam Silver',
          currency: 'EUR',
        },
      },
      {
        __typename: 'TransactionsByAccountIdEdge',
        node: {
          __typename: 'Transaction',
          id: '2eafca9c-31c4-40a5-b8a0-e9c5a58903c2',
          valueDate: '2026-07-18T00:00:00.000Z',
          amount: -11.7,
          payer: 'DM DROGERIE SAGT DANKE',
          payee: 'Kate Silver',
          currency: 'EUR',
        },
      },
      {
        __typename: 'TransactionsByAccountIdEdge',
        node: {
          __typename: 'Transaction',
          id: '1be18729-d928-4218-a894-f23de6c18b9b',
          valueDate: '2026-07-17T00:00:00.000Z',
          amount: -24.0,
          payer: 'Girokonto',
          payee: 'Kate and Adam Silver',
          currency: 'EUR',
        },
      },
      {
        __typename: 'TransactionsByAccountIdEdge',
        node: {
          __typename: 'Transaction',
          id: 'cc1cae6e-aba7-44e9-b05a-57f0bdeeea06',
          valueDate: '2026-07-14T00:00:00.000Z',
          amount: -3.99,
          payer: 'REWE',
          payee: 'Adam Silver',
          currency: 'EUR',
        },
      },
      {
        __typename: 'TransactionsByAccountIdEdge',
        node: {
          __typename: 'Transaction',
          id: 'bd20137d-aeb4-4212-bbeb-8bb58dcb6fae',
          valueDate: '2026-07-14T00:00:00.000Z',
          amount: -11.36,
          payer: 'REWE',
          payee: 'Adam Silver',
          currency: 'EUR',
        },
      },
      {
        __typename: 'TransactionsByAccountIdEdge',
        node: {
          __typename: 'Transaction',
          id: 'f909c12b-1e62-4032-8a6d-b37894a6fbf7',
          valueDate: '2026-07-14T00:00:00.000Z',
          amount: -10.38,
          payer: 'Singhs Markt',
          payee: 'Adam Silver',
          currency: 'EUR',
        },
      },
      {
        __typename: 'TransactionsByAccountIdEdge',
        node: {
          __typename: 'Transaction',
          id: '335646d7-1556-4b31-9651-a3b70b847157',
          valueDate: '2026-07-12T00:00:00.000Z',
          amount: -40.09,
          payer: 'ALDI SUED',
          payee: 'Kate Silver',
          currency: 'EUR',
        },
      },
      {
        __typename: 'TransactionsByAccountIdEdge',
        node: {
          __typename: 'Transaction',
          id: 'ce20dd98-411d-4e35-aae1-a00018cecdc6',
          valueDate: '2026-07-11T00:00:00.000Z',
          amount: -26.25,
          payer: 'KAUFLAND KOELN KALK 22',
          payee: 'Adam Silver',
          currency: 'EUR',
        },
      },
      {
        __typename: 'TransactionsByAccountIdEdge',
        node: {
          __typename: 'Transaction',
          id: '9ec7dc22-f60b-4d32-904d-efad52d0a209',
          valueDate: '2026-07-11T00:00:00.000Z',
          amount: -6.89,
          payer: 'ALDI SUED',
          payee: 'Kate Silver',
          currency: 'EUR',
        },
      },
      {
        __typename: 'TransactionsByAccountIdEdge',
        node: {
          __typename: 'Transaction',
          id: '82d724bd-69a2-4402-b24d-36f650dc7cf3',
          valueDate: '2026-07-10T00:00:00.000Z',
          amount: -13.73,
          payer: 'KAUFLAND KOELN KALK 22',
          payee: 'Kate Silver',
          currency: 'EUR',
        },
      },
      {
        __typename: 'TransactionsByAccountIdEdge',
        node: {
          __typename: 'Transaction',
          id: '7488bb86-3697-4d5e-934e-225f78f2fb11',
          valueDate: '2026-07-10T00:00:00.000Z',
          amount: -6.9,
          payer: 'DM DROGERIE SAGT DANKE',
          payee: 'Kate Silver',
          currency: 'EUR',
        },
      },
      {
        __typename: 'TransactionsByAccountIdEdge',
        node: {
          __typename: 'Transaction',
          id: '97234aef-91fd-4049-b446-cbd6cd742ad8',
          valueDate: '2026-07-10T00:00:00.000Z',
          amount: -26.78,
          payer: 'ALDI SUED',
          payee: 'Kate Silver',
          currency: 'EUR',
        },
      },
      {
        __typename: 'TransactionsByAccountIdEdge',
        node: {
          __typename: 'Transaction',
          id: 'e079c445-9ef9-4231-8db7-6767a2b851ed',
          valueDate: '2026-07-09T00:00:00.000Z',
          amount: -14.27,
          payer: 'KAUFLAND KOELN KALK 22',
          payee: 'Adam Silver',
          currency: 'EUR',
        },
      },
      {
        __typename: 'TransactionsByAccountIdEdge',
        node: {
          __typename: 'Transaction',
          id: 'f4c6ecf7-78e6-46bd-8741-c100ccd545c5',
          valueDate: '2026-07-07T00:00:00.000Z',
          amount: -38.24,
          payer: 'KAUFLAND KOELN KALK 22',
          payee: 'Kate Silver',
          currency: 'EUR',
        },
      },
      {
        __typename: 'TransactionsByAccountIdEdge',
        node: {
          __typename: 'Transaction',
          id: '2d5c9470-7993-4b19-ba18-383dcfb901d6',
          valueDate: '2026-07-07T00:00:00.000Z',
          amount: -22.32,
          payer: 'REWE',
          payee: 'Adam Silver',
          currency: 'EUR',
        },
      },
      {
        __typename: 'TransactionsByAccountIdEdge',
        node: {
          __typename: 'Transaction',
          id: 'e6e63f90-03b1-4f5e-b2ca-9ae01e9f04d8',
          valueDate: '2026-07-05T00:00:00.000Z',
          amount: -16.65,
          payer: 'KAUFLAND KOELN KALK 22',
          payee: 'Adam Silver',
          currency: 'EUR',
        },
      },
      {
        __typename: 'TransactionsByAccountIdEdge',
        node: {
          __typename: 'Transaction',
          id: '76c805e3-9926-4d95-b482-e2518eb73c3d',
          valueDate: '2026-07-05T00:00:00.000Z',
          amount: -2.59,
          payer: 'KAUFLAND KOELN KALK 22',
          payee: 'Adam Silver',
          currency: 'EUR',
        },
      },
      {
        __typename: 'TransactionsByAccountIdEdge',
        node: {
          __typename: 'Transaction',
          id: '97202668-4fc8-44dc-90e0-12ef24fa6fa1',
          valueDate: '2026-07-05T00:00:00.000Z',
          amount: 300.0,
          payer: 'C24 Gemeinschaftskonto',
          payee: 'Kate Silver',
          currency: 'EUR',
        },
      },
      {
        __typename: 'TransactionsByAccountIdEdge',
        node: {
          __typename: 'Transaction',
          id: 'b088d131-f2f7-40b5-ba5f-6015a1ab811c',
          valueDate: '2026-07-04T00:00:00.000Z',
          amount: -14.15,
          payer: 'KAUFLAND KOELN KALK 22',
          payee: 'Kate Silver',
          currency: 'EUR',
        },
      },
      {
        __typename: 'TransactionsByAccountIdEdge',
        node: {
          __typename: 'Transaction',
          id: 'bd5c3fc8-5723-4d5d-8ec5-2a463fa2d232',
          valueDate: '2026-07-03T00:00:00.000Z',
          amount: -9.35,
          payer: 'S-mart Lebensmit.',
          payee: 'Adam Silver',
          currency: 'EUR',
        },
      },
    ],
  },
} as unknown as GetTransactionsByAccountIdQuery;
