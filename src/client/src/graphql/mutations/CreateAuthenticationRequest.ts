import { gql, TypedDocumentNode } from '@apollo/client';
import { CreateAuthenticationRequestMutation } from '../types/graphql';

export const CreateAuthenticationRequest: TypedDocumentNode<CreateAuthenticationRequestMutation> =
  gql(`
  mutation CreateAuthenticationRequest(
    $institutionId: UUID!
    $redirectUri: String!
  ) {
    createAuthenticationRequest(
      input: { institutionId: $institutionId, redirectUri: $redirectUri }
    ) {
      authenticationRequest {
        id,
        externalId,
        associatedAccounts{
          id
        }
        status
        authenticationLink
      }
    }
  }
`);
