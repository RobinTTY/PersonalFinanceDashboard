import { ApolloLink, HttpLink } from '@apollo/client';
import { GraphQLWsLink } from '@apollo/client/link/subscriptions';
import { OperationTypeNode } from 'graphql';
import { createClient } from 'graphql-ws';
import { NEVER } from 'rxjs';
import { errorLink } from './errorLink';

function createSubscriptionLink(): ApolloLink {
  // The mocked API doesn't serve subscriptions
  if (import.meta.env.VITE_ENABLE_API_MOCKING === 'true') {
    return new ApolloLink(() => NEVER);
  }

  return new GraphQLWsLink(
    createClient({
      url: import.meta.env.VITE_API_URL.replace(/^http/, 'ws'),
      // Keeps reconnecting while the server restarts
      retryAttempts: Infinity,
    })
  );
}

/**
 * Sends subscriptions over a WebSocket and everything else over HTTP.
 */
export function createLink(): ApolloLink {
  return errorLink.concat(
    ApolloLink.split(
      (operation) => operation.operationType === OperationTypeNode.SUBSCRIPTION,
      createSubscriptionLink(),
      new HttpLink({ uri: import.meta.env.VITE_API_URL })
    )
  );
}
