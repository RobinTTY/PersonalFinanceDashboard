import { IconBuildingBank, IconChevronDown, IconPlus } from '@tabler/icons-react';
import { Avatar, Box, Group, Menu, Stack, Text, UnstyledButton } from '@mantine/core';
import { AuthRequestWithAccountsFragment } from '@graphql-types/graphql';
import { formatBalance, getInitials } from '@utility';
import classes from './AccountsHeader.module.css';

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
    <Box className={classes.header}>
      <Group justify="space-between" align="center" wrap="nowrap">
        <Box style={{ flex: 1 }} />

        <Menu position="bottom" withinPortal>
          <Menu.Target>
            <UnstyledButton
              disabled={!canSwitchAccount && accounts.length > 0}
              className={canSwitchAccount ? classes.accountButton : classes.accountButtonDisabled}
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
                  <IconChevronDown size={14} />
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
            <Menu.Divider />
            <Menu.Item color="blue" leftSection={<IconPlus size={14} />} onClick={onAddAccount}>
              Add account
            </Menu.Item>
          </Menu.Dropdown>
        </Menu>

        <Stack gap={2} align="flex-end" style={{ flex: 1 }}>
          <Text size="xs" fw={600} c="dimmed" tt="uppercase">
            Current balance
          </Text>
          <Text size="xl" fw={700}>
            {balanceLabel ?? '—'}
          </Text>
        </Stack>
      </Group>
    </Box>
  );
}
