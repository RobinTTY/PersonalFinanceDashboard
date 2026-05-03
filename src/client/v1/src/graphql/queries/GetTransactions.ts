import { gql } from '../types/gql';

export const GetTransactionsQuery = gql(`
  query GetTransactions($accountId: String!, $first: Int) {
    transactions(accountId: $accountId, first: $first) {
      pageInfo{
        hasNextPage
        hasPreviousPage
        startCursor
        endCursor
      }
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
