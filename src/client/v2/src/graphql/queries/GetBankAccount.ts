import { gql, TypedDocumentNode } from '@apollo/client';
import { BankAccountDetails } from '../fragments/BankAccountDetails';
import { GetBankAccountQuery, GetBankAccountQueryVariables } from '../types/graphql';

export const GetBankAccount: TypedDocumentNode<
  GetBankAccountQuery,
  GetBankAccountQueryVariables
> = gql`
  query GetBankAccount($accountId: UUID!) {
    bankAccount(accountId: $accountId) {
      ...BankAccountDetails
    }
  }
  ${BankAccountDetails}
`;
