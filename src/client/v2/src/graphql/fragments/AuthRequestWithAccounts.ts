import { gql, TypedDocumentNode } from '@apollo/client';
import { AuthRequestWithAccountsFragment } from '../types/graphql';

export const AuthRequestWithAccounts: TypedDocumentNode<AuthRequestWithAccountsFragment> = gql`
  fragment AuthRequestWithAccounts on AuthenticationRequest {
    id
    thirdPartyId
    status
    authenticationLink
    createdAt
    associatedAccounts {
      id
      thirdPartyId
      iban
      name
      description
      accountType
      balance
      currency
      ownerName
      associatedInstitution {
        name
        bic
        logoUri
      }
    }
  }
`;
