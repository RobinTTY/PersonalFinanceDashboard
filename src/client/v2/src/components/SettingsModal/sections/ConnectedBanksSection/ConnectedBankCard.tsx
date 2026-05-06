import { IconBuildingBank, IconTrash } from '@tabler/icons-react';
import { ActionIcon, Avatar, Badge, Divider, Group, Stack, Text, Tooltip, Accordion } from '@mantine/core';
import { AuthenticationStatus, AuthRequestWithAccountsFragment } from '@graphql-types/graphql';
import { formatCreatedAt, formatBalance, getInitials } from '@utility';
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

/**
 * Sub-component to render individual linked accounts within a connected bank card.
 * @param account The bank account to display details for.
 */
function AccountRow({ account }: { account: ConnectedBankAccount }) {
  const balance = formatBalance(account);
  const title = account.name || account.iban || 'Unnamed account';

  return (
    <div className={classes.accountCard}>
      <Stack gap={2} mb="xs">
        <Text fw={600} size="sm">
          {title}
        </Text>
        {account.iban && account.name && (
          <Text size="xs" c="dimmed">
            {account.iban}
          </Text>
        )}
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
      </Stack>
    </div>
  );
}

interface ConnectedBankCardProps {
  bank: AuthRequestWithAccounts;
  onRequestDelete: (bank: AuthRequestWithAccounts) => void;
  isDeleting: boolean;
}

export function ConnectedBankCard({ bank, onRequestDelete, isDeleting }: ConnectedBankCardProps) {
  const institution = bank.associatedAccounts[0]?.associatedInstitution;
  const status = bank.status;
  const accountCount = bank.associatedAccounts.length;

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
              Connected on {formatCreatedAt(bank.createdAt)}
            </Text>
          </Stack>
        </Group>

        <Group gap="xs" wrap="nowrap" flex="0 0 auto">
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
          <Accordion variant="filled" chevronPosition="right" classNames={{ item: classes.accordionItem }}>
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
