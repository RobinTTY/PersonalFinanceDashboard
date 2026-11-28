import { createClient, type ClientOptions } from 'graphql-ws';
import { createLink } from './link';

vi.mock('graphql-ws', () => ({
  createClient: vi.fn(() => ({ subscribe: vi.fn(), dispose: vi.fn() })),
}));

function connect(wasRetry: boolean) {
  const options = vi.mocked(createClient).mock.calls[0][0] as ClientOptions;
  const connected = options.on?.connected as (
    socket: unknown,
    payload: unknown,
    wasRetry: boolean
  ) => void;
  connected({}, undefined, wasRetry);
}

describe('createLink', () => {
  beforeEach(() => {
    vi.stubEnv('VITE_ENABLE_API_MOCKING', 'false');
    vi.mocked(createClient).mockClear();
  });

  afterEach(() => {
    vi.unstubAllEnvs();
  });

  it('reports when the WebSocket connected again', () => {
    const onReconnected = vi.fn();
    createLink({ onReconnected });

    connect(true);

    expect(onReconnected).toHaveBeenCalledTimes(1);
  });

  it('connects the WebSocket to the origin of the page for a relative API URL', () => {
    vi.stubEnv('VITE_API_URL', '/graphql');

    createLink();

    const options = vi.mocked(createClient).mock.calls[0][0] as ClientOptions;
    expect(options.url).toBe(`ws://${window.location.host}/graphql`);
  });

  it('keeps an absolute API URL', () => {
    vi.stubEnv('VITE_API_URL', 'https://api.example.com/graphql');

    createLink();

    const options = vi.mocked(createClient).mock.calls[0][0] as ClientOptions;
    expect(options.url).toBe('wss://api.example.com/graphql');
  });

  it('does not report the first connection', () => {
    const onReconnected = vi.fn();
    createLink({ onReconnected });

    connect(false);

    expect(onReconnected).not.toHaveBeenCalled();
  });
});
