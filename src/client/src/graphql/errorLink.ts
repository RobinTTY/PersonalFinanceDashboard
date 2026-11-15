import type { ErrorLike } from '@apollo/client';
import { CombinedGraphQLErrors, CombinedProtocolErrors, ServerError } from '@apollo/client/errors';
import { ErrorLink } from '@apollo/client/link/error';
import { notifications } from '@mantine/notifications';

declare module '@apollo/client' {
  interface DefaultContext {
    /**
     * Whether a failed operation shows an error notification. Set to `false` for operations whose
     * component renders the error itself. Defaults to `true`.
     */
    notifyOnError?: boolean;
  }
}

function getErrorMessages(error: ErrorLike): string[] {
  if (CombinedGraphQLErrors.is(error) || CombinedProtocolErrors.is(error)) {
    return [...new Set(error.errors.map(({ message }) => message))];
  }
  if (ServerError.is(error)) {
    return [`The server responded with status ${error.statusCode}.`];
  }
  return ['The server could not be reached. Please check your connection and try again.'];
}

/**
 * Shows a notification for every failed GraphQL operation, unless the operation opts out via the
 * `notifyOnError` context option.
 */
export const errorLink = new ErrorLink(({ error, operation }) => {
  if (operation.getContext().notifyOnError === false) {
    return;
  }

  for (const message of getErrorMessages(error)) {
    notifications.show({
      // Identical messages (e.g. several queries failing while offline) are only shown once
      id: `graphql-error:${message}`,
      color: 'red',
      title: 'Something went wrong',
      message,
    });
  }
});
