import { gql, TypedDocumentNode } from '@apollo/client';
import { TransactionFragment } from '../types/graphql';

export const Transaction: TypedDocumentNode<TransactionFragment> = gql`
  fragment Transaction on Transaction {
    id
    valueDate
    amount
    payer
    payee
    currency
  }
`;
