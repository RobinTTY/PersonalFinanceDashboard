import { gql, TypedDocumentNode } from '@apollo/client';
import { GetNetWorthHistoryQuery } from '../types/graphql';

export const GetNetWorthHistory: TypedDocumentNode<GetNetWorthHistoryQuery> = gql`
  query GetNetWorthHistory {
    bankAccounts(first: 50) {
      nodes {
        id
        balance
        currency
        transactions {
          valueDate
          amount
        }
      }
    }
  }
`;
