import { gql, TypedDocumentNode } from '@apollo/client';
import { AuthRequestWithAccountsFragment } from '../types/graphql';

export const AuthRequestWithAccounts: TypedDocumentNode<AuthRequestWithAccountsFragment> = gql`
  fragment AuthRequestWithAccounts on AuthenticationRequest {
    id
    externalId
    status
    authenticationLink
    createdAt
    accessExpiresAt
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
        id
        name
        bic
        logoUri
      }
      syncStatus {
        state
        lastSuccessfulSync
        lastError
        nextRetry
      }
    }
  }
`;
