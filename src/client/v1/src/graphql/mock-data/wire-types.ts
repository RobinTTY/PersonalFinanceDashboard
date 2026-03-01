import { Modify } from '@/utility/types/Modify';
import {
  GetTransactionsQuery,
  Transaction,
  TransactionsConnection,
  TransactionsEdge,
} from '../types/graphql';

/**
 * Data as returned from the GraphQL API, where the Date is represented as a string instead of the Date type.
 */
export type GetTransactionsQueryWire = Modify<
  GetTransactionsQuery,
  {
    transactions: Modify<
      TransactionsConnection,
      { edges: Modify<TransactionsEdge, { node: Modify<Transaction, { valueDate: string }> }>[] }
    >;
  }
>;
