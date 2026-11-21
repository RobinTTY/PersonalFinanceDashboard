import { SyncState } from '@graphql-types/graphql';
import { axe, render, screen, userEvent } from '@test-utils';
import { SyncStatusDot, SyncStatusIndicator } from './SyncStatusIndicator';

const HOUR = 60 * 60 * 1000;

const upToDate = {
  state: SyncState.UpToDate,
  lastSuccessfulSync: new Date(Date.now() - 3 * HOUR),
  lastError: null,
  nextRetry: null,
};

const retrying = {
  state: SyncState.Retrying,
  lastSuccessfulSync: new Date(Date.now() - 26 * HOUR),
  lastError: 'Rate limit exceeded',
  nextRetry: new Date(Date.now() + 2 * HOUR),
};

const needsReauthentication = {
  state: SyncState.NeedsReauthentication,
  lastSuccessfulSync: null,
  lastError: null,
  nextRetry: null,
};

const notSynced = {
  state: SyncState.NotSynced,
  lastSuccessfulSync: null,
  lastError: null,
  nextRetry: null,
};

describe('SyncStatusIndicator', () => {
  axe([
    <SyncStatusIndicator key="1" status={upToDate} />,
    <SyncStatusIndicator key="2" status={retrying} />,
    <SyncStatusIndicator key="3" status={needsReauthentication} />,
    <SyncStatusDot key="4" status={retrying} />,
  ]);

  it('shows when an up to date account was synced', () => {
    render(<SyncStatusIndicator status={upToDate} />);
    expect(screen.getByText('Synced 3 hours ago')).toBeInTheDocument();
  });

  it('shows that an account was never synced', () => {
    render(<SyncStatusIndicator status={{ ...upToDate, lastSuccessfulSync: null }} />);
    expect(screen.getByText('Not synced yet')).toBeInTheDocument();
  });

  it('shows when a failed sync is retried, and the error on hover', async () => {
    render(<SyncStatusIndicator status={retrying} />);
    expect(screen.getByText('Sync failed · retrying in 2 hours')).toBeInTheDocument();

    await userEvent.hover(screen.getByText(/Sync failed/));
    expect(await screen.findByText('Rate limit exceeded')).toBeInTheDocument();
    expect(screen.getByText('Synced yesterday')).toBeInTheDocument();
  });

  it('shows that a due retry happens soon', () => {
    render(
      <SyncStatusIndicator status={{ ...retrying, nextRetry: new Date(Date.now() - HOUR) }} />
    );
    expect(screen.getByText('Sync failed · retrying soon')).toBeInTheDocument();
  });

  it('asks to reconnect the bank when the access expired', () => {
    render(<SyncStatusIndicator status={needsReauthentication} />);
    expect(screen.getByText('Reconnect bank')).toBeInTheDocument();
  });

  it('shows nothing for manual accounts', () => {
    // The container also holds Mantine's style elements, so it's never empty
    const { container } = render(<SyncStatusIndicator status={notSynced} />);
    expect(container.querySelector(':not(style)')).toBeNull();
  });
});

describe('SyncStatusDot', () => {
  it('marks accounts that need attention', () => {
    render(<SyncStatusDot status={needsReauthentication} />);
    expect(screen.getByRole('img', { name: 'Reconnect bank' })).toBeInTheDocument();
  });

  it('shows nothing for accounts that are up to date', () => {
    render(<SyncStatusDot status={upToDate} />);
    expect(screen.queryByRole('img')).not.toBeInTheDocument();
  });
});
