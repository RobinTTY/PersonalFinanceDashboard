import { gql } from "@apollo/client";

export const CreateAuthenticationRequestQuery = gql`
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
`;
