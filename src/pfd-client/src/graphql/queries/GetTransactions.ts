import { gql } from '../types/gql';

export const GetTransactionsQuery = gql(`
  query GetTransactions($accountId: String!) {
    transactions(accountId: $accountId) {
      edges {
        node {
          valueDate
          payer
          payee
          amount
          currency
          category
          tags
          notes
        }
      }
    }
  }
`);
