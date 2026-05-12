import { IconBuildingBank, IconChevronDown, IconPlus } from '@tabler/icons-react';
import { Avatar, Box, Button, Group, Menu, Stack, Text, UnstyledButton } from '@mantine/core';
import { AuthRequestWithAccountsFragment } from '@graphql-types/graphql';
import { formatBalance, getInitials } from '@utility';

type Account = AuthRequestWithAccountsFragment['associatedAccounts'][number];

interface AccountsHeaderProps {
  currentAccount: Account | null;
  accounts: Account[];
  loadingAccounts: boolean;
  onAccountChange: (accountId: string) => void;
  onAddAccount: () => void;
}

export function AccountsHeader({
  currentAccount,
  accounts,
  loadingAccounts,
  onAccountChange,
  onAddAccount,
}: AccountsHeaderProps) {
  const institution = currentAccount?.associatedInstitution;
  const accountLabel =
    currentAccount?.name ?? currentAccount?.iban ?? (loadingAccounts ? 'Loading…' : 'No account');
  const balanceLabel = currentAccount ? formatBalance(currentAccount) : null;
  const canSwitchAccount = accounts.length > 1;

  return (
    <Box
      style={{
        background: 'light-dark(var(--mantine-color-gray-0), var(--mantine-color-dark-6))',
        borderRadius: 'var(--mantine-radius-md)',
        padding: 'var(--mantine-spacing-sm) var(--mantine-spacing-md)',
        boxShadow: '0 2px 4px rgba(0, 0, 0, 0.2)',
        border: '.5px solid light-dark(var(--mantine-color-gray-3), var(--mantine-color-dark-4))',
      }}
    >
      <Group justify="space-between" align="center" wrap="nowrap">
        <Stack gap={2} style={{ flex: 1, minWidth: 0 }}>
          <Text size="xs" fw={600} c="dimmed" tt="uppercase">
            Current balance
          </Text>
          <Text size="xl" fw={700}>
            {balanceLabel ?? '—'}
          </Text>
        </Stack>

        <Menu position="bottom" withinPortal disabled={!canSwitchAccount}>
          <Menu.Target>
            <UnstyledButton
              disabled={!canSwitchAccount}
              style={{ cursor: canSwitchAccount ? 'pointer' : 'default' }}
            >
              <Stack align="center" gap={6}>
                <Avatar
                  src={institution?.logoUri ? String(institution.logoUri) : undefined}
                  radius="xl"
                  size="lg"
                >
                  {institution ? getInitials(institution.name) : <IconBuildingBank size={20} />}
                </Avatar>
                <Group gap={4} wrap="nowrap" align="center">
                  <Text fw={600} size="sm">
                    {accountLabel}
                  </Text>
                  {canSwitchAccount && <IconChevronDown size={14} />}
                </Group>
              </Stack>
            </UnstyledButton>
          </Menu.Target>
          <Menu.Dropdown>
            {accounts.map((account) => (
              <Menu.Item
                key={String(account.id)}
                onClick={() => onAccountChange(String(account.id))}
              >
                {account.name ?? account.iban ?? 'Unnamed account'}
              </Menu.Item>
            ))}
          </Menu.Dropdown>
        </Menu>

        <Group justify="flex-end" style={{ flex: 1 }}>
          <Button leftSection={<IconPlus size={16} />} onClick={onAddAccount}>
            Add Account
          </Button>
        </Group>
      </Group>
    </Box>
  );
}
