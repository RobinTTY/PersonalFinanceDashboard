import { gql } from '../types/gql';

export const CreateAuthenticationRequestMutation = gql(`
  mutation CreateAuthenticationRequest(
    $institutionId: String!
    $redirectUri: String!
  ) {
    createAuthenticationRequest(
      institutionId: $institutionId
      redirectUri: $redirectUri
    ) {
      id
      associatedAccounts
      status
      authenticationLink
    }
  }
`);
