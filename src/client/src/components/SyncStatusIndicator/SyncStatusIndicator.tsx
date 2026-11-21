import { SyncState, SyncStatus } from '@graphql-types/graphql';
import { IconPlugConnectedX, IconRefreshAlert } from '@tabler/icons-react';
import { formatRelativeTime } from '@utility';
import { Badge, Box, Stack, Text, Tooltip } from '@mantine/core';

type Status = Pick<SyncStatus, 'state' | 'lastSuccessfulSync' | 'lastError' | 'nextRetry'>;

interface SyncStatusProps {
  status: Status;
}

interface SyncStatusIndicatorProps extends SyncStatusProps {
  /** Called when the user wants to reconnect a bank whose access expired. Without it, nothing is clickable. */
  onReconnect?: () => void;
}

function formatLastSync(lastSuccessfulSync: Date | null) {
  return lastSuccessfulSync ? `Synced ${formatRelativeTime(lastSuccessfulSync)}` : 'Not synced yet';
}

function formatRetry(nextRetry: Date | null) {
  // A due retry happens with the next refresh
  return nextRetry && nextRetry > new Date()
    ? `retrying ${formatRelativeTime(nextRetry)}`
    : 'retrying soon';
}

/**
 * Shows how up to date an account is with its bank: when it was synced, or that syncing failed or
 * needs the bank to be reconnected.
 */
export function SyncStatusIndicator({ status, onReconnect }: SyncStatusIndicatorProps) {
  switch (status.state) {
    case SyncState.UpToDate:
      return (
        <Text size="xs" c="dimmed">
          {formatLastSync(status.lastSuccessfulSync)}
        </Text>
      );
    case SyncState.Retrying:
      return (
        <Tooltip
          multiline
          w={300}
          withArrow
          events={{ hover: true, focus: true, touch: true }}
          label={
            <Stack gap={4}>
              <Text size="xs">{status.lastError ?? 'The bank could not be reached.'}</Text>
              <Text size="xs" c="dimmed">
                {formatLastSync(status.lastSuccessfulSync)}
              </Text>
            </Stack>
          }
        >
          <Badge
            color="yellow"
            variant="light"
            size="sm"
            tt="none"
            tabIndex={0}
            leftSection={<IconRefreshAlert size={12} />}
          >
            Sync failed · {formatRetry(status.nextRetry)}
          </Badge>
        </Tooltip>
      );
    case SyncState.NeedsReauthentication:
      return (
        <Tooltip
          multiline
          w={300}
          withArrow
          events={{ hover: true, focus: true, touch: true }}
          label={`The access to this bank expired, so the account isn't synced anymore. ${formatLastSync(status.lastSuccessfulSync)}.`}
        >
          <Badge
            color="red"
            variant="light"
            size="sm"
            tt="none"
            leftSection={<IconPlugConnectedX size={12} />}
            {...(onReconnect
              ? { component: 'button', onClick: onReconnect, style: { cursor: 'pointer' } }
              : { tabIndex: 0 })}
          >
            Reconnect bank
          </Badge>
        </Tooltip>
      );
    default:
      return null;
  }
}

const DOT_COLORS: Partial<Record<SyncState, { color: string; label: string }>> = {
  [SyncState.Retrying]: { color: 'var(--mantine-color-yellow-6)', label: 'Sync failed' },
  [SyncState.NeedsReauthentication]: {
    color: 'var(--mantine-color-red-6)',
    label: 'Reconnect bank',
  },
};

/**
 * A small dot marking accounts whose sync needs attention, nothing for accounts that are fine.
 */
export function SyncStatusDot({ status }: SyncStatusProps) {
  const dot = DOT_COLORS[status.state];
  if (!dot) {
    return null;
  }

  return (
    <Box
      component="span"
      role="img"
      aria-label={dot.label}
      w={8}
      h={8}
      bg={dot.color}
      style={{ borderRadius: '50%', display: 'inline-block' }}
    />
  );
}
