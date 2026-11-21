import { SyncState } from '@graphql-types/graphql';
import { axe, render, screen, userEvent } from '@test-utils';
import { OpenSettingsProvider } from '../SettingsModal/OpenSettingsContext';
import { SyncStatusAlert } from './SyncStatusAlert';

function renderAlert(states: SyncState[], openSettings = vi.fn()) {
  return render(
    <OpenSettingsProvider value={openSettings}>
      <SyncStatusAlert states={states} />
    </OpenSettingsProvider>
  );
}

describe('SyncStatusAlert', () => {
  axe([
    <OpenSettingsProvider key="1" value={vi.fn()}>
      <SyncStatusAlert states={[SyncState.Retrying, SyncState.NeedsReauthentication]} />
    </OpenSettingsProvider>,
  ]);

  it('shows nothing while all accounts are fine', () => {
    renderAlert([SyncState.UpToDate, SyncState.NotSynced]);
    expect(screen.queryByRole('alert')).not.toBeInTheDocument();
  });

  it('warns about accounts whose sync is retried', () => {
    renderAlert([SyncState.UpToDate, SyncState.Retrying]);
    expect(screen.getByRole('alert')).toHaveTextContent(
      "1 account couldn't be synced and will be retried, so the figures below may be outdated."
    );
  });

  it('names accounts that need the bank to be reconnected first', () => {
    renderAlert([
      SyncState.Retrying,
      SyncState.NeedsReauthentication,
      SyncState.NeedsReauthentication,
    ]);
    expect(screen.getByRole('alert')).toHaveTextContent(
      "2 accounts can't be synced until the bank is reconnected, and 1 account couldn't be synced and will be retried"
    );
  });

  it('opens the connected banks settings', async () => {
    const openSettings = vi.fn();
    renderAlert([SyncState.Retrying], openSettings);
    await userEvent.click(screen.getByRole('button', { name: 'Manage bank connections' }));
    expect(openSettings).toHaveBeenCalledWith('Connected Banks');
  });

  it('offers no settings link outside the app layout', () => {
    render(<SyncStatusAlert states={[SyncState.Retrying]} />);
    expect(
      screen.queryByRole('button', { name: 'Manage bank connections' })
    ).not.toBeInTheDocument();
  });

  it('can be dismissed', async () => {
    renderAlert([SyncState.Retrying]);
    await userEvent.click(screen.getByRole('button', { name: 'Dismiss' }));
    expect(screen.queryByRole('alert')).not.toBeInTheDocument();
  });
});
