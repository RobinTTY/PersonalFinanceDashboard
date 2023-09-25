import { gql } from '../types/gql';

export const GetAccountQuery = gql(`
  query GetAccount($accountId: String!) {
    account(accountId: $accountId) {
      name
      description
      balance
      currency
      iban
      bic
      bban
      ownerName
      accountType
    }
  }
`);
