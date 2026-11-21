import { gql, TypedDocumentNode } from '@apollo/client';
import { AuthRequestWithAccounts } from '../fragments/AuthRequestWithAccounts';
import { GetAuthRequestWithAccountsQuery } from '../types/graphql';

export const GetAuthRequestWithAccounts: TypedDocumentNode<GetAuthRequestWithAccountsQuery> = gql`
  query GetAuthRequestWithAccounts($authenticationId: UUID!) {
    # Retrieves the request from the bank and, once access was granted, its accounts
    authenticationRequest(authenticationId: $authenticationId, refresh: true) {
      ...AuthRequestWithAccounts
    }
  }
  ${AuthRequestWithAccounts}
`;
