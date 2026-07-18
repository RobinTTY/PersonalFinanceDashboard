import { Group, Stack, Text, ThemeIcon, Title } from '@mantine/core';
import { IconCircleCheck } from '@tabler/icons-react';
import { BankAccountOverview } from '../BankAccountOverview/BankAccountOverview';

interface BankAuthenticationSuccessProps {
  /** Internal ids of the accounts linked by the completed authentication request. */
  accountIds: string[];
}

export function BankAuthenticationSuccess({ accountIds }: BankAuthenticationSuccessProps) {
  const accountCount = accountIds.length;

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
      {accountCount > 0 && (
        <>
          <Text size="sm" c="dimmed" ta="center">
            Choose which accounts to include in your analytics. You can change this later in the
            connected banks settings.
          </Text>
          <BankAccountOverview accountIds={accountIds} />
        </>
      )}
    </Stack>
  );
}
