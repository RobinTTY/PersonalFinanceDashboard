import { ApolloClient, gql, InMemoryCache } from '@apollo/client';
import { MockLink } from '@apollo/client/testing';
import { refetchSynchronizedQueries } from './refetchSynchronizedQueries';

const BALANCE = gql`
  query Balance {
    balance
  }
`;
const AUTHENTICATION_STATUS = gql`
  query AuthenticationStatus {
    status
  }
`;

describe('refetchSynchronizedQueries', () => {
  it('refetches the active queries except those that opted out', async () => {
    const requests = { balance: 0, status: 0 };
    const client = new ApolloClient({
      cache: new InMemoryCache(),
      link: new MockLink([
        {
          request: { query: BALANCE },
          result: () => ({ data: { balance: ++requests.balance } }),
          maxUsageCount: 2,
        },
        {
          request: { query: AUTHENTICATION_STATUS },
          result: () => ({ data: { status: ++requests.status } }),
          maxUsageCount: 2,
        },
      ]),
    });
    const balance = client.watchQuery({ query: BALANCE }).subscribe(() => {});
    const status = client
      .watchQuery({
        query: AUTHENTICATION_STATUS,
        context: { refetchOnDataSynchronized: false },
      })
      .subscribe(() => {});
    await vi.waitFor(() => expect(requests).toEqual({ balance: 1, status: 1 }));

    await refetchSynchronizedQueries(client);

    expect(requests).toEqual({ balance: 2, status: 1 });
    balance.unsubscribe();
    status.unsubscribe();
  });
});
