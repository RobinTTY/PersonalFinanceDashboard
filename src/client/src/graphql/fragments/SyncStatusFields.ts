import { gql, TypedDocumentNode } from '@apollo/client';
import { SyncStatusFieldsFragment } from '../types/graphql';

export const SyncStatusFields: TypedDocumentNode<SyncStatusFieldsFragment> = gql`
  fragment SyncStatusFields on SyncStatus {
    state
    lastSuccessfulSync
    lastError
    nextRetry
  }
`;
