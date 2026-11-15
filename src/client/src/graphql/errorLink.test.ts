import { ApolloClient, ApolloLink, gql, InMemoryCache } from '@apollo/client';
import { ServerError } from '@apollo/client/errors';
import { Observable, throwError } from 'rxjs';
import type { MockInstance } from 'vitest';
import { notifications } from '@mantine/notifications';
import { errorLink } from './errorLink';

const TestMutation = gql`
  mutation Test {
    test
  }
`;

function createClient(terminatingLink: ApolloLink) {
  return new ApolloClient({ link: errorLink.concat(terminatingLink), cache: new InMemoryCache() });
}

const graphQLErrorLink = (...messages: string[]) =>
  new ApolloLink(
    () =>
      new Observable((observer) => {
        observer.next({ data: null, errors: messages.map((message) => ({ message })) });
        observer.complete();
      })
  );

describe('errorLink', () => {
  let show: MockInstance<typeof notifications.show>;

  beforeEach(() => {
    show = vi.spyOn(notifications, 'show').mockReturnValue('');
  });

  afterEach(() => vi.restoreAllMocks());

  it('shows each distinct GraphQL error message once', async () => {
    const client = createClient(graphQLErrorLink('Not found.', 'Not found.', 'Invalid id.'));

    await expect(client.mutate({ mutation: TestMutation })).rejects.toThrow();

    expect(show.mock.calls.map(([notification]) => notification.message)).toEqual([
      'Not found.',
      'Invalid id.',
    ]);
  });

  it('shows the status code of a server error', async () => {
    const response = new Response('', { status: 503 });
    const client = createClient(
      new ApolloLink(() => throwError(() => new ServerError('', { response, bodyText: '' })))
    );

    await expect(client.mutate({ mutation: TestMutation })).rejects.toThrow();

    expect(show).toHaveBeenCalledWith(
      expect.objectContaining({ message: 'The server responded with status 503.' })
    );
  });

  it('shows a connection error when the server is unreachable', async () => {
    const client = createClient(
      new ApolloLink(() => throwError(() => new TypeError('Failed to fetch')))
    );

    await expect(client.mutate({ mutation: TestMutation })).rejects.toThrow();

    expect(show).toHaveBeenCalledWith(
      expect.objectContaining({ message: expect.stringContaining('could not be reached') })
    );
  });

  it('shows nothing for operations that opt out', async () => {
    const client = createClient(graphQLErrorLink('Not found.'));

    await expect(
      client.mutate({ mutation: TestMutation, context: { notifyOnError: false } })
    ).rejects.toThrow();

    expect(show).not.toHaveBeenCalled();
  });
});
