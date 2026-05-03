import { Group, Stack, Text, ThemeIcon, Title } from '@mantine/core';
import { IconCircleCheck } from '@tabler/icons-react';
import {
  AssociatedAccount,
  BankAccountOverview,
} from '../BankAccountOverview/BankAccountOverview';

interface BankAuthenticationSuccessProps {
  accounts: AssociatedAccount[];
}

export function BankAuthenticationSuccess({ accounts }: BankAuthenticationSuccessProps) {
  const accountCount = accounts.length;

  return (
    <Stack h="100%" align="center" gap="lg">
      <Group gap="sm" align="center">
        <ThemeIcon color="green" size={40} radius="xl">
          <IconCircleCheck size={22} />
        </ThemeIcon>
        <Stack gap={2}>
          <Title order={5}>Authentication Successful</Title>
          <Text size="sm" c="dimmed">
            {accountCount === 1
              ? 'Your bank account has been successfully linked.'
              : accountCount > 1
                ? `${accountCount} bank accounts have been successfully linked.`
                : 'Your bank has been successfully linked.'}
          </Text>
        </Stack>
      </Group>
      {accounts.length > 0 && <BankAccountOverview accounts={accounts} />}
    </Stack>
  );
}
