import { gql } from '../types/gql';

export const GetAuthenticationRequestQuery = gql(`
  query GetAuthenticationRequest($authenticationId: String!) {
    authenticationRequest(authenticationId: $authenticationId) {
      id
      associatedAccounts
      status
      authenticationLink
    }
  }
`);
