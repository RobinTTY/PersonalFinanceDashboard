import { SyncState } from '@graphql-types/graphql';
import { Stack } from '@mantine/core';
import { OpenSettingsProvider } from '../SettingsModal/OpenSettingsContext';
import { ComponentPreview } from '../storybook/ComponentPreview';
import { SyncStatusAlert } from './SyncStatusAlert';
import { SyncStatusDot, SyncStatusIndicator } from './SyncStatusIndicator';

const HOUR = 60 * 60 * 1000;

export default { title: 'Accounts/SyncStatus' };

export function Indicator() {
  return (
    <ComponentPreview canvas={{ center: false }} withSpacing>
      <Stack align="flex-start">
        <SyncStatusIndicator
          status={{
            state: SyncState.UpToDate,
            lastSuccessfulSync: new Date(Date.now() - 2 * HOUR),
            lastError: null,
            nextRetry: null,
          }}
        />
        <SyncStatusIndicator
          status={{
            state: SyncState.Retrying,
            lastSuccessfulSync: new Date(Date.now() - 26 * HOUR),
            lastError: 'Rate limit exceeded. Too many requests',
            nextRetry: new Date(Date.now() + 2 * HOUR),
          }}
        />
        <SyncStatusIndicator
          status={{
            state: SyncState.NeedsReauthentication,
            lastSuccessfulSync: new Date(Date.now() - 72 * HOUR),
            lastError: null,
            nextRetry: null,
          }}
        />
        <SyncStatusDot
          status={{
            state: SyncState.Retrying,
            lastSuccessfulSync: null,
            lastError: null,
            nextRetry: null,
          }}
        />
      </Stack>
    </ComponentPreview>
  );
}

export function Alert() {
  return (
    <OpenSettingsProvider value={() => {}}>
      <ComponentPreview canvas={{ center: false }} withSpacing>
        <SyncStatusAlert
          states={[SyncState.UpToDate, SyncState.Retrying, SyncState.NeedsReauthentication]}
        />
      </ComponentPreview>
    </OpenSettingsProvider>
  );
}
