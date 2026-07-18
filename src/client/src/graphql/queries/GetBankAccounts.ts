import { gql, TypedDocumentNode } from '@apollo/client';
import { BankAccountSummary } from '../fragments/BankAccountSummary';
import { GetBankAccountsQuery } from '../types/graphql';

export const GetBankAccounts: TypedDocumentNode<GetBankAccountsQuery> = gql`
  query GetBankAccounts {
    bankAccounts(first: 50, where: { includeInAnalytics: { eq: true } }) {
      nodes {
        ...BankAccountSummary
      }
    }
  }
  ${BankAccountSummary}
`;
