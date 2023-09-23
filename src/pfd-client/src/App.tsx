import '@mantine/core/styles.css';
import { MantineProvider } from '@mantine/core';
import { Router } from './Router';
import { theme } from './theme';
import { ApolloProvider, ApolloClient, InMemoryCache } from '@apollo/client';
import { MockedProvider } from '@apollo/client/testing';
import { MockData } from './mock-data/api/MockData';

// TODO: Remove when fixed: https://github.com/mantinedev/mantine/issues/4830
import './App.css';

const testEnvironment = true;

// Appolo (GraphQL) Setup
const client = new ApolloClient({
  uri: 'http://localhost:5115/graphql',
  cache: new InMemoryCache(),
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
