import { gql, TypedDocumentNode } from '@apollo/client';
import { GetAuthenticationRequestQuery } from '../types/graphql';

export const GetAuthenticationRequest: TypedDocumentNode<GetAuthenticationRequestQuery> = gql(`
  query GetAuthenticationRequest($authenticationId: UUID!) {
    authenticationRequest(authenticationId: $authenticationId) {
      id
      thirdPartyId
      status
      authenticationLink
      associatedAccounts {
        id
      }
    }
  }
`);
