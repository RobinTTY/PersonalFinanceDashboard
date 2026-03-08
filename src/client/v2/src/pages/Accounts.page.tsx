import { Button, Group, Stack, Title } from '@mantine/core';
import { IconPlus } from '@tabler/icons-react';

export function AccountsPage() {
  return (
    <Stack>
      <Group justify="space-between">
        <Title order={2}>Accounts</Title>
        <Button leftSection={<IconPlus size={16} />}>Add Account</Button>
      </Group>
    </Stack>
  );
}
