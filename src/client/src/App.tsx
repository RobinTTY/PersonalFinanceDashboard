import '@mantine/core/styles.css';
import { MantineProvider } from '@mantine/core';
import { ApolloProvider, ApolloClient, InMemoryCache } from '@apollo/client';
import { Router } from './Router';
import { scalarTypePolicies } from './graphql/types/type-policies';
import { theme } from './theme';

// TODO: Investigate caching (https://www.apollographql.com/docs/react/caching/cache-configuration/)
// Appolo (GraphQL) Setup
const client = new ApolloClient({
  uri: 'http://localhost:5115/graphql',
  cache: new InMemoryCache({ typePolicies: scalarTypePolicies }),
  connectToDevTools: true,
});

export const App = () => (
  <ApolloProvider client={client}>
    <MantineProvider theme={theme} defaultColorScheme="auto">
      <Router />
    </MantineProvider>
  </ApolloProvider>
);
