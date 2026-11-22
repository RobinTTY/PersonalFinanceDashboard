import {
  AuthenticationStatus,
  AuthRequestWithAccountsFragment,
  SyncState,
} from '@graphql-types/graphql';
import { IconBuildingBank, IconPlugConnected, IconTrash } from '@tabler/icons-react';
import { formatBalance, formatRelativeTime, getInitials } from '@utility';
import {
  Accordion,
  ActionIcon,
  Avatar,
  Badge,
  Button,
  Divider,
  Group,
  Stack,
  Text,
  Tooltip,
} from '@mantine/core';
import { ReconnectInstitution } from '@/components/AddAccountModal/AddAccountModal';
import { IncludeInAnalyticsSwitch } from '@/components/IncludeInAnalyticsSwitch/IncludeInAnalyticsSwitch';
import { SyncStatusIndicator } from '@/components/SyncStatusIndicator/SyncStatusIndicator';
import classes from './ConnectedBanksSection.module.css';

type AuthRequestWithAccounts = AuthRequestWithAccountsFragment;
type ConnectedBankAccount = AuthRequestWithAccounts['associatedAccounts'][number];

const STATUS_COLORS: Record<AuthenticationStatus, string> = {
  [AuthenticationStatus.Active]: 'green',
  [AuthenticationStatus.Pending]: 'yellow',
  [AuthenticationStatus.RequiresUserAction]: 'orange',
  [AuthenticationStatus.Expired]: 'red',
  [AuthenticationStatus.Failed]: 'red',
  [AuthenticationStatus.Unknown]: 'gray',
};

const STATUS_LABELS: Record<AuthenticationStatus, string> = {
  [AuthenticationStatus.Active]: 'Active',
  [AuthenticationStatus.Pending]: 'Pending',
  [AuthenticationStatus.RequiresUserAction]: 'Requires action',
  [AuthenticationStatus.Expired]: 'Expired',
  [AuthenticationStatus.Failed]: 'Failed',
  [AuthenticationStatus.Unknown]: 'Unknown',
};

// Connections expiring sooner than this are highlighted, so they can be renewed in time
const EXPIRY_WARNING_DAYS = 14;
const DAY = 24 * 60 * 60 * 1000;

/**
 * Gets the institution of a connection. Connections stored before their institution was are
 * identified by their linked accounts.
 */
export function getConnectionInstitution(bank: AuthRequestWithAccounts) {
  return bank.institution ?? bank.associatedAccounts[0]?.associatedInstitution;
}

/**
 * Shows when the access of an active or expired connection ends, highlighted once it's close.
 */
function AccessExpiry({ bank }: { bank: AuthRequestWithAccounts }) {
  if (bank.status !== AuthenticationStatus.Active && bank.status !== AuthenticationStatus.Expired) {
    return null;
  }

  const expiresAt = bank.accessExpiresAt;
  const remaining = expiresAt.getTime() - Date.now();
  if (remaining <= 0) {
    return (
      <Text size="xs" c="red">
        Access expired {formatRelativeTime(expiresAt)}
      </Text>
    );
  }

  return (
    <Text size="xs" c={remaining < EXPIRY_WARNING_DAYS * DAY ? 'orange' : 'dimmed'}>
      Access expires {formatRelativeTime(expiresAt)}
    </Text>
  );
}

/**
 * Sub-component to render individual linked accounts within a connected bank card.
 * @param account The bank account to display details for.
 */
function AccountRow({ account }: { account: ConnectedBankAccount }) {
  const balance = formatBalance(account);
  const title = account.name || account.iban || 'Unnamed account';

  return (
    <div className={classes.accountCard}>
      <Stack gap={2} mb="xs" align="flex-start">
        <Text fw={600} size="sm">
          {title}
        </Text>
        {account.iban && account.name && (
          <Text size="xs" c="dimmed">
            {account.iban}
          </Text>
        )}
        <SyncStatusIndicator status={account.syncStatus} />
      </Stack>
      <Stack gap="xs">
        {account.accountType && (
          <Group justify="space-between" wrap="nowrap">
            <Text size="xs" c="dimmed">
              Type
            </Text>
            <Badge variant="light" size="sm">
              {account.accountType}
            </Badge>
          </Group>
        )}
        {balance && (
          <Group justify="space-between" wrap="nowrap">
            <Text size="xs" c="dimmed">
              Balance
            </Text>
            <Text size="sm" fw={600}>
              {balance}
            </Text>
          </Group>
        )}
        {account.ownerName && (
          <Group justify="space-between" wrap="nowrap">
            <Text size="xs" c="dimmed">
              Owner
            </Text>
            <Text size="sm" fw={500}>
              {account.ownerName}
            </Text>
          </Group>
        )}
        {account.id != null && (
          <>
            <Divider my={4} />
            <IncludeInAnalyticsSwitch
              accountId={String(account.id)}
              includeInAnalytics={account.includeInAnalytics}
              size="xs"
            />
          </>
        )}
      </Stack>
    </div>
  );
}

interface ConnectedBankCardProps {
  bank: AuthRequestWithAccounts;
  onRequestDelete: (bank: AuthRequestWithAccounts) => void;
  onReconnect: (institution: ReconnectInstitution) => void;
  isDeleting: boolean;
}

export function ConnectedBankCard({
  bank,
  onRequestDelete,
  onReconnect,
  isDeleting,
}: ConnectedBankCardProps) {
  const institution = getConnectionInstitution(bank);
  const status = bank.status;
  const accountCount = bank.associatedAccounts.length;
  // Once a newer connection took over the accounts, this one doesn't need to be renewed anymore
  const needsReconnect = bank.associatedAccounts.some(
    (account) => account.syncStatus.state === SyncState.NeedsReauthentication
  );

  return (
    <div className={classes.bankCard}>
      <Group justify="space-between" align="flex-start" wrap="nowrap">
        <Group gap="md" wrap="nowrap" style={{ minWidth: 0, flex: 1 }}>
          <Avatar
            src={institution?.logoUri ? String(institution.logoUri) : undefined}
            radius="sm"
            size="md"
            flex="0 0 auto"
          >
            {institution ? getInitials(institution.name) : <IconBuildingBank size={18} />}
          </Avatar>
          <Stack gap={2} style={{ minWidth: 0 }}>
            <Text fw={600} size="sm" truncate>
              {institution?.name ?? 'Unknown institution'}
            </Text>
            <Text size="xs" c="dimmed">
              Connected on{' '}
              {bank.createdAt.toLocaleDateString(undefined, {
                year: 'numeric',
                month: 'short',
                day: 'numeric',
              })}
            </Text>
            <AccessExpiry bank={bank} />
          </Stack>
        </Group>

        <Group gap="xs" wrap="nowrap" flex="0 0 auto">
          {needsReconnect && institution?.id != null && (
            <Button
              size="compact-xs"
              variant="light"
              leftSection={<IconPlugConnected size={14} />}
              onClick={() => onReconnect({ id: String(institution.id), name: institution.name })}
            >
              Reconnect
            </Button>
          )}
          <Badge variant="light" color={STATUS_COLORS[status]} size="sm">
            {STATUS_LABELS[status]}
          </Badge>
          <Tooltip label="Remove connection" withArrow>
            <ActionIcon
              variant="subtle"
              color="red"
              loading={isDeleting}
              onClick={() => onRequestDelete(bank)}
              aria-label={`Delete connection to ${institution?.name ?? 'bank'}`}
            >
              <IconTrash size={16} />
            </ActionIcon>
          </Tooltip>
        </Group>
      </Group>

      {accountCount > 0 && (
        <>
          <Divider my="sm" />
          <Accordion
            variant="filled"
            chevronPosition="right"
            classNames={{ item: classes.accordionItem }}
          >
            <Accordion.Item value="accounts">
              <Accordion.Control>
                <Text size="sm" fw={500}>
                  {accountCount === 1 ? '1 linked account' : `${accountCount} linked accounts`}
                </Text>
              </Accordion.Control>
              <Accordion.Panel>
                <Stack gap="sm">
                  {bank.associatedAccounts.map((account) => (
                    <AccountRow key={String(account.id)} account={account} />
                  ))}
                </Stack>
              </Accordion.Panel>
            </Accordion.Item>
          </Accordion>
        </>
      )}
    </div>
  );
}
