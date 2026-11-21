import type { ApolloClient } from '@apollo/client';

declare module '@apollo/client' {
  interface DefaultContext {
    /**
     * Whether an active query is refetched after the server synchronized data with a bank. Set to `false` for
     * queries that ask the bank themselves. Defaults to `true`.
     */
    refetchOnDataSynchronized?: boolean;
  }
}

/**
 * Refetches the active queries after the server synchronized data with a bank, so balances, transactions and
 * sync statuses stay current.
 */
export function refetchSynchronizedQueries(client: ApolloClient) {
  return client.refetchQueries({
    include: 'active',
    onQueryUpdated: (query) => query.options.context?.refetchOnDataSynchronized !== false,
  });
}
