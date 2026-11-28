import { ApolloLink, HttpLink } from '@apollo/client';
import { GraphQLWsLink } from '@apollo/client/link/subscriptions';
import { OperationTypeNode } from 'graphql';
import { createClient } from 'graphql-ws';
import { NEVER } from 'rxjs';
import { errorLink } from './errorLink';

interface LinkOptions {
  /** Called when the WebSocket connected again after it was lost, e.g. while the server restarted. */
  onReconnected?: () => void;
}

function createSubscriptionLink({ onReconnected }: LinkOptions): ApolloLink {
  // The mocked API doesn't serve subscriptions
  if (import.meta.env.VITE_ENABLE_API_MOCKING === 'true') {
    return new ApolloLink(() => NEVER);
  }

  // Resolved against the page, since the API can be served from the same origin under a relative URL
  const apiUrl = new URL(import.meta.env.VITE_API_URL, window.location.href);

  return new GraphQLWsLink(
    createClient({
      url: apiUrl.href.replace(/^http/, 'ws'),
      // Keeps reconnecting while the server restarts
      retryAttempts: Infinity,
      on: {
        connected: (_socket, _payload, wasRetry) => {
          if (wasRetry) {
            onReconnected?.();
          }
        },
      },
    })
  );
}

/**
 * Sends subscriptions over a WebSocket and everything else over HTTP.
 */
export function createLink(options: LinkOptions = {}): ApolloLink {
  return errorLink.concat(
    ApolloLink.split(
      (operation) => operation.operationType === OperationTypeNode.SUBSCRIPTION,
      createSubscriptionLink(options),
      new HttpLink({ uri: import.meta.env.VITE_API_URL })
    )
  );
}
