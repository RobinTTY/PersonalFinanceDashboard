import '@mantine/core/styles.css';
import '@mantine/notifications/styles.css';

import { ApolloClient } from '@apollo/client';
import { ApolloProvider } from '@apollo/client/react';
import { MantineProvider } from '@mantine/core';
import { Notifications } from '@mantine/notifications';
import { createCache } from './graphql/cache';
import { createLink } from './graphql/link';
import { refetchSynchronizedQueries } from './graphql/refetchSynchronizedQueries';
import { Router } from './Router';
import { theme } from './theme';

const client: ApolloClient = new ApolloClient({
  // Synchronizations that completed while disconnected are refetched
  link: createLink({ onReconnected: () => void refetchSynchronizedQueries(client) }),
  cache: createCache(),
  devtools: {
    enabled: import.meta.env.DEV,
  },
});

export default function App() {
  return (
    <ApolloProvider client={client}>
      <MantineProvider theme={theme} defaultColorScheme="auto">
        <Notifications />
        <Router />
      </MantineProvider>
    </ApolloProvider>
  );
}
