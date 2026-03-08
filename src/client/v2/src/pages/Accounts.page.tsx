import { Button, Group, Stack, Title } from '@mantine/core';
import { useDisclosure } from '@mantine/hooks';
import { IconPlus } from '@tabler/icons-react';
import { AddAccountModal } from '@/components/AddAccountModal/AddAccountModal';

export function AccountsPage() {
  const [addAccountOpened, { open: openAddAccount, close: closeAddAccount }] =
    useDisclosure(false);

  return (
    <Stack>
      <Group justify="space-between">
        <Title order={2} m={0}>Accounts</Title>
        <Button leftSection={<IconPlus size={16} />} onClick={openAddAccount}>
          Add Account
        </Button>
      </Group>

      <AddAccountModal opened={addAccountOpened} onClose={closeAddAccount} />
    </Stack>
  );
}
