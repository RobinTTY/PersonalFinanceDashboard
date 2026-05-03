import { gql, TypedDocumentNode } from '@apollo/client';
import { AuthRequestWithAccounts } from '../fragments/AuthRequestWithAccounts';
import { GetAuthRequestsWithAccountsQuery } from '../types/graphql';

export const GetAuthRequestsWithAccounts: TypedDocumentNode<GetAuthRequestsWithAccountsQuery> = gql`
  query GetAuthRequestsWithAccounts {
    authenticationRequests {
      ...AuthRequestWithAccounts
    }
  }
  ${AuthRequestWithAccounts}
`;
