import { useState } from 'react';
import { SyncState } from '@graphql-types/graphql';
import { IconPlugConnectedX, IconRefreshAlert } from '@tabler/icons-react';
import { Alert, Anchor, Text } from '@mantine/core';
import { useOpenSettings } from '@/components/SettingsModal/OpenSettingsContext';

interface SyncStatusAlertProps {
  /** The sync states of the accounts shown on the page. */
  states: SyncState[];
}

function countAccounts(count: number) {
  return count === 1 ? '1 account' : `${count} accounts`;
}

/**
 * Warns that some accounts aren't up to date, so the data on the page may be outdated. Renders nothing
 * while all accounts are fine.
 */
export function SyncStatusAlert({ states }: SyncStatusAlertProps) {
  const [dismissed, setDismissed] = useState(false);
  const openSettings = useOpenSettings();
  const needsReauthentication = states.filter(
    (state) => state === SyncState.NeedsReauthentication
  ).length;
  const retrying = states.filter((state) => state === SyncState.Retrying).length;

  if (dismissed || needsReauthentication + retrying === 0) {
    return null;
  }

  const problems = [
    needsReauthentication > 0 &&
      `${countAccounts(needsReauthentication)} can't be synced until the bank is reconnected`,
    retrying > 0 && `${countAccounts(retrying)} couldn't be synced and will be retried`,
  ].filter(Boolean);

  return (
    <Alert
      color={needsReauthentication > 0 ? 'red' : 'yellow'}
      icon={
        needsReauthentication > 0 ? (
          <IconPlugConnectedX size={16} />
        ) : (
          <IconRefreshAlert size={16} />
        )
      }
      title="Some accounts aren't up to date"
      withCloseButton
      closeButtonLabel="Dismiss"
      onClose={() => setDismissed(true)}
    >
      <Text size="sm">
        {problems.join(', and ')}, so the figures below may be outdated.{' '}
        {openSettings && (
          <Anchor component="button" size="sm" onClick={() => openSettings('Connected Banks')}>
            Manage bank connections
          </Anchor>
        )}
      </Text>
    </Alert>
  );
}
