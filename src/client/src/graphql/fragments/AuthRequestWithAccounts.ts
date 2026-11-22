import { gql, TypedDocumentNode } from '@apollo/client';
import { AuthRequestWithAccountsFragment } from '../types/graphql';
import { SyncStatusFields } from './SyncStatusFields';

export const AuthRequestWithAccounts: TypedDocumentNode<AuthRequestWithAccountsFragment> = gql`
  fragment AuthRequestWithAccounts on AuthenticationRequest {
    id
    externalId
    status
    authenticationLink
    createdAt
    accessExpiresAt
    institution {
      id
      name
      bic
      logoUri
    }
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
        ...SyncStatusFields
      }
    }
  }
  ${SyncStatusFields}
`;
