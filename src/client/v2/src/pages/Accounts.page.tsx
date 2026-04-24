import { useEffect, useState } from 'react';
import { IconPlus } from '@tabler/icons-react';
import { Button, Group, Stack, Title } from '@mantine/core';
import { useDisclosure } from '@mantine/hooks';
import { AddAccountModal, PendingAuthState } from '@/components/AddAccountModal/AddAccountModal';

export function AccountsPage() {
  const [addAccountOpened, { open: openAddAccount, close: closeAddAccount }] = useDisclosure(false);
  const [pendingAuth, setPendingAuth] = useState<PendingAuthState | undefined>(undefined);

  useEffect(() => {
    const stored = localStorage.getItem('pendingGoCardlessAuth');
    if (stored) {
      localStorage.removeItem('pendingGoCardlessAuth');
      try {
        setPendingAuth(JSON.parse(stored) as PendingAuthState);
        openAddAccount();
      } catch {}
    }
  }, [openAddAccount]);

  const handleModalClose = () => {
    setPendingAuth(undefined);
    closeAddAccount();
  };

  return (
    <Stack>
      <Group justify="space-between">
        <Title order={2} m={0}>
          Accounts
        </Title>
        <Button leftSection={<IconPlus size={16} />} onClick={openAddAccount}>
          Add Account
        </Button>
      </Group>

      <AddAccountModal opened={addAccountOpened} onClose={handleModalClose} pendingAuth={pendingAuth} />
    </Stack>
  );
}
