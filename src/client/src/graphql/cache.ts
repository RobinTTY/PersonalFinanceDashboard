import { InMemoryCache } from '@apollo/client';
import { scalarTypePolicies } from './types/type-policies';

/**
 * Creates the Apollo cache with the type policies the app relies on.
 */
export function createCache() {
  return new InMemoryCache({
    typePolicies: {
      ...scalarTypePolicies,
      BankAccount: {
        fields: {
          // The sync status has no id, so queries selecting only some of its fields would otherwise replace it
          syncStatus: { merge: true },
        },
      },
      Query: {
        fields: {
          // Cursor pagination: cache one entry per accountId (treating first/after/last/before as
          // pagination controls, not identity), and append incoming edges to existing ones so
          // fetchMore grows the list instead of replacing it.
          transactionsByAccountId: {
            keyArgs: ['accountId'],
            merge(existing, incoming, { args }) {
              if (!existing || !args?.after) {
                return incoming;
              }
              return {
                ...incoming,
                edges: [...(existing.edges ?? []), ...(incoming.edges ?? [])],
              };
            },
          },
        },
      },
    },
  });
}
