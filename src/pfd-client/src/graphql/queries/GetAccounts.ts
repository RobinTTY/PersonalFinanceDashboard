import { gql } from '../types/gql';

// Apollo uses offset pagination by default: https://www.apollographql.com/docs/react/pagination/core-api/
// HotChocolate recommends using Connections: https://chillicream.com/docs/hotchocolate/v13/fetching-data/pagination/#connections
// TODO: Figure out what would be the best strategy
export const GetAccountsQuery = gql(`
  query GetAccounts($accountIds: [String!]!) {
    accounts(accountIds: $accountIds) {
      edges {
        node {
          id
          name
          description
          balance
          currency
          iban
          bic
          bban
          ownerName
          accountType
        }
      }
    }
  }
`);

export const GetMinimalAccountsQuery = gql(`
  query GetMinimalAccounts($accountIds: [String!]!) {
    accounts(accountIds: $accountIds) {
      edges {
        node {
          description
          balance
          currency
        }
      }
    }
  }
`);
