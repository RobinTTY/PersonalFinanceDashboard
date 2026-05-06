import { useState } from 'react';
import { useMutation, useQuery } from '@apollo/client/react';
import { IconAlertCircle, IconBuildingBank } from '@tabler/icons-react';
import { Alert, Button, Group, Loader, Modal, Stack, Text } from '@mantine/core';
import { GetAuthRequestsWithAccounts } from '@graphql-queries/GetAuthRequestsWithAccounts';
import { DeleteAuthenticationRequest } from '@graphql-mutations/DeleteAuthenticationRequest';
import { AuthRequestWithAccountsFragment } from '@graphql-types/graphql';
import { ConnectedBankCard } from './ConnectedBankCard';

type AuthRequestWithAccounts = AuthRequestWithAccountsFragment;

export function ConnectedBanksSection() {
  const { data, loading, error } = useQuery(GetAuthRequestsWithAccounts);
  const [pendingDeletion, setPendingDeletion] = useState<AuthRequestWithAccounts | null>(null);
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

  const banks = data?.authenticationRequests ?? [];
  const pendingInstitutionName =
    pendingDeletion?.associatedAccounts[0]?.associatedInstitution?.name ?? 'this bank';

  return (
    <Stack gap="lg">
      <Stack gap={4}>
        <Text size="sm">
          Manage the bank connections used to sync your accounts and transactions.
        </Text>
        <Text size="xs" c="dimmed">
          If an institution has multiple active connections, removing one will not affect account syncing as long as at least one connection remains.
        </Text>
      </Stack>

      {loading ? (
        <Stack gap="xs" align="center" py="xl">
          <Loader size="sm" />
          <Text size="sm" c="dimmed">
            Fetching your connections…
          </Text>
        </Stack>
      ) : error ? (
        <Alert color="red" icon={<IconAlertCircle size={16} />} title="Failed to load connected banks">
          {error.message}
        </Alert>
      ) : banks.length === 0 ? (
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
              onRequestDelete={(authRequest) => {
                setDeleteError(null);
                setPendingDeletion(authRequest);
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
        styles={{ title: { fontWeight: 600 } }}
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
