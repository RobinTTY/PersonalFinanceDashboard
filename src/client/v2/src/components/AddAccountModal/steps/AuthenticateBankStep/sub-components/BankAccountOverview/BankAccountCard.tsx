import { useEffect } from 'react';
import { useQuery } from '@apollo/client/react';
import { GetBankAccount } from '@/graphql/queries/GetBankAccount';
import { BankAccountDetailsFragment } from '@/graphql/types/graphql';
import { IconAlertCircle } from '@tabler/icons-react';
import { Badge, Divider, Group, Skeleton, Stack, Text } from '@mantine/core';
import { IncludeInAnalyticsSwitch } from '@/components/IncludeInAnalyticsSwitch/IncludeInAnalyticsSwitch';
import classes from './BankAccountOverview.module.css';

export type BankAccountInstitution = NonNullable<
  BankAccountDetailsFragment['associatedInstitution']
>;

interface BankAccountCardProps {
  /** Internal id of the bank account to load and display. */
  accountId: string;
  /** Reports the loaded institution up so the overview can render a single shared header. */
  onInstitutionLoaded?: (institution: BankAccountInstitution) => void;
}

/**
 * Loads a single bank account by its id and renders its details.
 */
export function BankAccountCard({ accountId, onInstitutionLoaded }: BankAccountCardProps) {
  const { data, loading, error } = useQuery(GetBankAccount, {
    variables: { accountId },
    fetchPolicy: 'network-only',
  });

  const account = data?.bankAccount;
  const institution = account?.associatedInstitution;

  useEffect(() => {
    if (institution) {
      onInstitutionLoaded?.(institution);
    }
  }, [institution, onInstitutionLoaded]);

  if (loading) {
    return (
      <div className={classes.accountCard}>
        <Stack gap="xs">
          <Skeleton height={18} width="55%" />
          <Skeleton height={14} width="35%" />
          <Skeleton height={14} width="45%" mt="xs" />
        </Stack>
      </div>
    );
  }

  if (error || !account) {
    return (
      <div className={classes.accountCard}>
        <Group gap="xs" wrap="nowrap">
          <IconAlertCircle size={18} color="var(--mantine-color-red-6)" />
          <Text size="sm" c="dimmed">
            This account couldn't be loaded. It will appear once syncing completes.
          </Text>
        </Group>
      </div>
    );
  }

  const hasNameOrDescription = account.name || account.description;
  const balanceValue =
    account.balance != null
      ? account.currency
        ? `${String(account.balance)} ${account.currency}`
        : String(account.balance)
      : null;

  return (
    <div className={classes.accountCard}>
      {hasNameOrDescription && (
        <Stack gap={2} mb="md">
          {account.name && (
            <Text fw={600} size="md">
              {account.name}
            </Text>
          )}
          {account.description && account.description !== account.name && (
            <Text size="sm" c="dimmed">
              {account.description}
            </Text>
          )}
        </Stack>
      )}

      <Stack gap="xs">
        {account.accountType && (
          <Group justify="space-between" wrap="nowrap">
            <Text size="sm" c="dimmed">
              Type
            </Text>
            <Badge variant="light" size="sm">
              {account.accountType}
            </Badge>
          </Group>
        )}
        {hasNameOrDescription && account.accountType && <Divider />}
        {balanceValue && (
          <Group justify="space-between" wrap="nowrap">
            <Text size="sm" c="dimmed">
              Balance
            </Text>
            <Text size="sm" fw={600}>
              {balanceValue}
            </Text>
          </Group>
        )}
        {account.ownerName && (
          <Group justify="space-between" wrap="nowrap">
            <Text size="sm" c="dimmed">
              Owner
            </Text>
            <Text size="sm" fw={500}>
              {account.ownerName}
            </Text>
          </Group>
        )}
      </Stack>

      {account.id != null && (
        <>
          <Divider my="sm" />
          <IncludeInAnalyticsSwitch
            accountId={String(account.id)}
            includeInAnalytics={account.includeInAnalytics}
          />
        </>
      )}
    </div>
  );
}
