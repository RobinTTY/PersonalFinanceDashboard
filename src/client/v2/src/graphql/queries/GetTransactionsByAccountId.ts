import { gql, TypedDocumentNode } from '@apollo/client';
import { Transaction } from '../fragments/Transaction';
import {
  GetTransactionsByAccountIdQuery,
  GetTransactionsByAccountIdQueryVariables,
} from '../types/graphql';

export const GetTransactionsByAccountId: TypedDocumentNode<
  GetTransactionsByAccountIdQuery,
  GetTransactionsByAccountIdQueryVariables
> = gql`
  query GetTransactionsByAccountId(
    $accountId: UUID!
    $first: Int
    $after: String
    $last: Int
    $before: String
  ) {
    transactionsByAccountId(
      accountId: $accountId
      first: $first
      after: $after
      last: $last
      before: $before
    ) {
      pageInfo {
        hasNextPage
        startCursor
        endCursor
      }
      edges {
        cursor
        node {
          ...Transaction
        }
      }
    }
  }
  ${Transaction}
`;
