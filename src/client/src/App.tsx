import '@mantine/core/styles.css';

import { ApolloClient, HttpLink, InMemoryCache } from '@apollo/client';
import { ApolloProvider } from '@apollo/client/react';
import { MantineProvider } from '@mantine/core';
import { scalarTypePolicies } from './graphql/types/type-policies';
import { Router } from './Router';
import { theme } from './theme';

const client = new ApolloClient({
  link: new HttpLink({ uri: 'http://localhost:5115/graphql' }),
  cache: new InMemoryCache({
    typePolicies: {
      ...scalarTypePolicies,
      Query: {
        fields: {
          // Cursor pagination: cache one entry per accountId (treating first/after/last/before as
          // pagination controls, not identity), and append incoming edges to existing ones so
          // fetchMore grows the list instead of replacing it.
          transactionsByAccountId: {
            keyArgs: ['accountId'],
            merge(existing, incoming) {
              if (!existing) {
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
  }),
  devtools: {
    enabled: true,
  },
});

export default function App() {
  return (
    <ApolloProvider client={client}>
      <MantineProvider theme={theme} defaultColorScheme="auto">
        <Router />
      </MantineProvider>
    </ApolloProvider>
  );
}
