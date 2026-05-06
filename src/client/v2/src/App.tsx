import '@mantine/core/styles.css';

import { ApolloClient, HttpLink, InMemoryCache } from '@apollo/client';
import { ApolloProvider } from '@apollo/client/react';
import { MantineProvider } from '@mantine/core';
import { scalarTypePolicies } from './graphql/types/type-policies';
import { Router } from './Router';
import { theme } from './theme';

const client = new ApolloClient({
  link: new HttpLink({ uri: 'http://localhost:5115/graphql' }),
  cache: new InMemoryCache({ typePolicies: scalarTypePolicies }),
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
