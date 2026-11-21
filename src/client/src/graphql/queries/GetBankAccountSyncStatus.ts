import { gql, TypedDocumentNode } from '@apollo/client';
import { SyncStatusFields } from '../fragments/SyncStatusFields';
import {
  GetBankAccountSyncStatusQuery,
  GetBankAccountSyncStatusQueryVariables,
} from '../types/graphql';

export const GetBankAccountSyncStatus: TypedDocumentNode<
  GetBankAccountSyncStatusQuery,
  GetBankAccountSyncStatusQueryVariables
> = gql`
  query GetBankAccountSyncStatus($accountId: UUID!) {
    bankAccount(accountId: $accountId) {
      id
      syncStatus {
        ...SyncStatusFields
      }
    }
  }
  ${SyncStatusFields}
`;
