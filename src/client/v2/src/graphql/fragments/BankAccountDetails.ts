import { gql, TypedDocumentNode } from '@apollo/client';
import { BankAccountDetailsFragment } from '../types/graphql';

export const BankAccountDetails: TypedDocumentNode<BankAccountDetailsFragment> = gql`
  fragment BankAccountDetails on BankAccount {
    id
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
`;
