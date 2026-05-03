import { useState } from 'react';
import { useMutation, useQuery } from '@apollo/client/react';
import { IconAlertCircle, IconBuildingBank, IconTrash } from '@tabler/icons-react';
import {
  Accordion,
  ActionIcon,
  Alert,
  Avatar,
  Badge,
  Button,
  Center,
  Divider,
  Group,
  Loader,
  Modal,
  Stack,
  Text,
  Tooltip,
} from '@mantine/core';
import { DeleteAuthenticationRequest } from '@/graphql/mutations/DeleteAuthenticationRequest';
import { GetAuthRequestsWithAccounts } from '@/graphql/queries/GetAuthRequestsWithAccounts';
import { AuthenticationStatus, GetAuthRequestsWithAccountsQuery } from '@/graphql/types/graphql';
import { getInitials } from '@/utility/getInitials';
import classes from './ConnectedBanksSection.module.css';

type ConnectedBank = GetAuthRequestsWithAccountsQuery['authenticationRequests'][number];
type ConnectedAccount = ConnectedBank['associatedAccounts'][number];

const STATUS_COLORS: Record<AuthenticationStatus, string> = {
  [AuthenticationStatus.Active]: 'green',
  [AuthenticationStatus.Pending]: 'yellow',
  [AuthenticationStatus.RequiresUserAction]: 'orange',
  [AuthenticationStatus.Expired]: 'gray',
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

function formatCreatedAt(value: unknown): string {
  if (!value) {
    return '';
  }
  const date = new Date(String(value));
  if (Number.isNaN(date.getTime())) {
    return String(value);
  }
  return date.toLocaleDateString(undefined, { year: 'numeric', month: 'short', day: 'numeric' });
}

function formatBalance(account: ConnectedAccount): string | null {
  if (account.balance == null) {
    return null;
  }
  return account.currency
    ? `${String(account.balance)} ${account.currency}`
    : String(account.balance);
}

function AccountRow({ account }: { account: ConnectedAccount }) {
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
  bank: ConnectedBank;
  onRequestDelete: (bank: ConnectedBank) => void;
  isDeleting: boolean;
}

function ConnectedBankCard({ bank, onRequestDelete, isDeleting }: ConnectedBankCardProps) {
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

export function ConnectedBanksSection() {
  const { data, loading, error } = useQuery(GetAuthRequestsWithAccounts);
  const [pendingDeletion, setPendingDeletion] = useState<ConnectedBank | null>(null);
  const [deleteError, setDeleteError] = useState<string | null>(null);
  const [deleteAuthenticationRequest, { loading: deleting }] = useMutation(
    DeleteAuthenticationRequest,
    { refetchQueries: [GetAuthRequestsWithAccounts] }
  );

  const closeConfirmation = () => {
    if (deleting) {
      return;
    }
    setPendingDeletion(null);
    setDeleteError(null);
  };

  const confirmDelete = async () => {
    if (!pendingDeletion?.id) {
      return;
    }

    try {
      await deleteAuthenticationRequest({
        variables: { authenticationId: pendingDeletion.id },
      });
      setPendingDeletion(null);
      setDeleteError(null);
    } catch (err) {
      setDeleteError(err instanceof Error ? err.message : 'Failed to delete connection.');
    }
  };

  if (loading) {
    return (
      <Center py="xl">
        <Loader size="sm" />
      </Center>
    );
  }

  if (error) {
    return (
      <Alert color="red" icon={<IconAlertCircle size={16} />} title="Failed to load connected banks">
        {error.message}
      </Alert>
    );
  }

  const banks = data?.authenticationRequests ?? [];
  const pendingInstitutionName =
    pendingDeletion?.associatedAccounts[0]?.associatedInstitution?.name ?? 'this bank';

  return (
    <Stack gap="lg">
      <Stack gap={4}>
        <Text size="sm">
          Manage the bank connections used to import your accounts and transactions.
        </Text>
        <Text size="xs" c="dimmed">
          If you have multiple active connections to the same institution, removing one of them will not prevent you from syncing the associated accounts, as long as at least one active connection to that institution remains.
        </Text>
      </Stack>

      {banks.length === 0 ? (
        <Stack gap="xs" align="center" py="xl">
          <IconBuildingBank size={32} color="var(--mantine-color-dimmed)" />
          <Text size="sm" c="dimmed">
            No connected banks yet.
          </Text>
        </Stack>
      ) : (
        <Stack gap="md">
          {banks.map((bank) => (
            <ConnectedBankCard
              key={String(bank.id)}
              bank={bank}
              isDeleting={deleting && pendingDeletion?.id === bank.id}
              onRequestDelete={(target) => {
                setDeleteError(null);
                setPendingDeletion(target);
              }}
            />
          ))}
        </Stack>
      )}

      <Modal
        opened={pendingDeletion !== null}
        onClose={closeConfirmation}
        title="Remove bank connection"
        centered
      >
        <Stack gap="md">
          <Text size="sm">
            Are you sure you want to remove the connection to{' '}
            <Text component="span" fw={600}>
              {pendingInstitutionName}
            </Text>
            ? This will stop syncing the associated accounts.
          </Text>
          {deleteError && (
            <Alert color="red" icon={<IconAlertCircle size={16} />}>
              {deleteError}
            </Alert>
          )}
          <Group justify="flex-end" gap="sm">
            <Button variant="default" onClick={closeConfirmation} disabled={deleting}>
              Cancel
            </Button>
            <Button color="red" onClick={confirmDelete} loading={deleting}>
              Remove
            </Button>
          </Group>
        </Stack>
      </Modal>
    </Stack>
  );
}
