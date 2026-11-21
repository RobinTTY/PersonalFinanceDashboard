import { useApolloClient, useSubscription } from '@apollo/client/react';
import { refetchSynchronizedQueries } from '@/graphql/refetchSynchronizedQueries';
import { DataSynchronized } from '@/graphql/subscriptions/DataSynchronized';

/**
 * Refetches the active queries whenever the server synchronized data with a bank, e.g. in the background or
 * right after a bank was connected, so balances, transactions and sync statuses stay current.
 */
export function useRefetchOnDataSynchronized() {
  const client = useApolloClient();

  useSubscription(DataSynchronized, {
    context: { notifyOnError: false },
    ignoreResults: true,
    onData: () => {
      void refetchSynchronizedQueries(client);
    },
  });
}
