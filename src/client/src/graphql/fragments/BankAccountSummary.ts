import { gql, TypedDocumentNode } from '@apollo/client';
import { BankAccountSummaryFragment } from '../types/graphql';

export const BankAccountSummary: TypedDocumentNode<BankAccountSummaryFragment> = gql`
  fragment BankAccountSummary on BankAccount {
    id
    iban
    name
    balance
    currency
    associatedInstitution {
      name
      logoUri
    }
  }
`;
