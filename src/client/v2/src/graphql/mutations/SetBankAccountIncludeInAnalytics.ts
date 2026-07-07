import { gql, TypedDocumentNode } from '@apollo/client';
import { SetBankAccountIncludeInAnalyticsMutation } from '../types/graphql';

export const SetBankAccountIncludeInAnalytics: TypedDocumentNode<SetBankAccountIncludeInAnalyticsMutation> =
  gql(`
  mutation SetBankAccountIncludeInAnalytics(
    $bankAccountId: UUID!
    $includeInAnalytics: Boolean!
  ) {
    setBankAccountIncludeInAnalytics(
      input: { bankAccountId: $bankAccountId, includeInAnalytics: $includeInAnalytics }
    ) {
      bankAccount {
        id
        includeInAnalytics
      }
    }
  }
`);
