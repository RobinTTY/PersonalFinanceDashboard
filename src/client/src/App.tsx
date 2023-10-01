import '@mantine/core/styles.css';
import { MantineProvider } from '@mantine/core';
import { ApolloProvider, ApolloClient, InMemoryCache } from '@apollo/client';
import { MockedProvider } from '@apollo/client/testing';
import { Router } from './Router';
import { theme } from './theme';

import { MockData } from './graphql/mock-data/MockData';
import { scalarTypePolicies } from './graphql/types/type-policies';

// TODO: This could be used to provide a demo mode without backend
const testEnvironment = true;

// TODO: Investigate caching (https://www.apollographql.com/docs/react/caching/cache-configuration/)
// Appolo (GraphQL) Setup
const client = new ApolloClient({
  uri: 'http://localhost:5115/graphql',
  cache: new InMemoryCache({ typePolicies: scalarTypePolicies }),
  connectToDevTools: true,
});

export default function App() {
  return testEnvironment ? (
    <MockedProvider mocks={MockData} addTypename={false}>
      <MantineProvider theme={theme} defaultColorScheme="auto">
        <Router />
      </MantineProvider>
    </MockedProvider>
  ) : (
    <ApolloProvider client={client}>
      <MantineProvider theme={theme} defaultColorScheme="auto">
        <Router />
      </MantineProvider>
    </ApolloProvider>
  );
}
