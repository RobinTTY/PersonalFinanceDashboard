import { gql } from '../graphql-types/gql';

export const CreateAuthenticationRequestQuery = gql(`
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
