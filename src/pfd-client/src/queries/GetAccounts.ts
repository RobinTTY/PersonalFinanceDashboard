import { gql } from '@apollo/client';

// Apollo uses offset pagination by default: https://www.apollographql.com/docs/react/pagination/core-api/
// HotChocolate recommends using Connections: https://chillicream.com/docs/hotchocolate/v13/fetching-data/pagination/#connections
// TODO: Figure out what would be the best strategy
export const GetAccountsQuery = gql`
  query GetAccounts($first: Int) {
    accounts(first: $first) {
      edges {
        node {
          description
          balance
          currency
        }
      }
    }
  }
`;
