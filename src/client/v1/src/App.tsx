import '@mantine/core/styles.css';
import { MantineProvider } from '@mantine/core';
import { ApolloClient, InMemoryCache, HttpLink } from '@apollo/client';
import { Defer20220824Handler } from "@apollo/client/incremental";
import { LocalState } from "@apollo/client/local-state";
import { ApolloProvider } from "@apollo/client/react";
import { Router } from './Router';
import { scalarTypePolicies } from './graphql/types/type-policies';
import { theme } from './theme';

// TODO: Investigate caching (https://www.apollographql.com/docs/react/caching/cache-configuration/)
// Appolo (GraphQL) Setup
const client = new ApolloClient({
  cache: new InMemoryCache({ typePolicies: scalarTypePolicies }),

  link: new HttpLink({
    uri: 'http://localhost:5115/graphql'
  }),

  /*
  Inserted by Apollo Client 3->4 migration codemod.
  If you are not using the `@client` directive in your application,
  you can safely remove this option.
  */
  localState: new LocalState({}),

  devtools: {
    enabled: true
  },

  /*
  Inserted by Apollo Client 3->4 migration codemod.
  If you are not using the `@defer` directive in your application,
  you can safely remove this option.
  */
  incrementalHandler: new Defer20220824Handler()
});

export const App = () => (
  <ApolloProvider client={client}>
    <MantineProvider theme={theme} defaultColorScheme="auto">
      <Router />
    </MantineProvider>
  </ApolloProvider>
);
