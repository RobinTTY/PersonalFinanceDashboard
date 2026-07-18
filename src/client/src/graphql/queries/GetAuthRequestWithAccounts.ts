import { gql, TypedDocumentNode } from '@apollo/client';
import { AuthRequestWithAccounts } from '../fragments/AuthRequestWithAccounts';
import { GetAuthRequestWithAccountsQuery } from '../types/graphql';

export const GetAuthRequestWithAccounts: TypedDocumentNode<GetAuthRequestWithAccountsQuery> = gql`
  query GetAuthRequestWithAccounts($authenticationId: UUID!) {
    authenticationRequest(authenticationId: $authenticationId) {
      ...AuthRequestWithAccounts
    }
  }
  ${AuthRequestWithAccounts}
`;
