import { gql, TypedDocumentNode } from '@apollo/client';
import { CreateAuthenticationRequestMutation } from '../types/graphql';

export const CreateAuthenticationRequest: TypedDocumentNode<CreateAuthenticationRequestMutation> = gql(`
  mutation CreateAuthenticationRequest(
    $institutionId: String!
    $redirectUri: String!
  ) {
    createAuthenticationRequest(
      input: { institutionId: $institutionId, redirectUri: $redirectUri }
    ) {
      authenticationRequest {
        id,
        thirdPartyId,
        associatedAccounts{
          id
        }
        status
        authenticationLink
      }
    }
  }
`);
