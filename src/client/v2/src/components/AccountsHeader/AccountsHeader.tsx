import { IconBuildingBank, IconChevronDown, IconPlus } from '@tabler/icons-react';
import { Avatar, Box, Button, Group, Menu, Stack, Text, UnstyledButton } from '@mantine/core';
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
        <Stack gap={2} className={classes.balanceStack}>
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

        <Group justify="flex-end" className={classes.actionsGroup}>
          <Button leftSection={<IconPlus size={16} />} onClick={onAddAccount}>
            Add Account
          </Button>
        </Group>
      </Group>
    </Box>
  );
}
