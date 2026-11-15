import { gql, TypedDocumentNode } from '@apollo/client';
import { AuthRequestWithAccountsFragment } from '../types/graphql';

export const AuthRequestWithAccounts: TypedDocumentNode<AuthRequestWithAccountsFragment> = gql`
  fragment AuthRequestWithAccounts on AuthenticationRequest {
    id
    externalId
    status
    authenticationLink
    createdAt
    associatedAccounts {
      id
      externalId
      iban
      name
      description
      accountType
      balance
      currency
      ownerName
      includeInAnalytics
      associatedInstitution {
        name
        bic
        logoUri
      }
    }
  }
`;
