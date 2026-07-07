import { Avatar, Badge, Divider, Group, Stack, Text } from '@mantine/core';
import { IncludeInAnalyticsSwitch } from '@/components/IncludeInAnalyticsSwitch/IncludeInAnalyticsSwitch';
import { GetAuthRequestWithAccountsQuery } from '@/graphql/types/graphql';
import { getInitials } from '@/utility/getInitials';
import classes from './BankAccountOverview.module.css';

export type AssociatedAccount = NonNullable<
  NonNullable<
    GetAuthRequestWithAccountsQuery['authenticationRequest']
  >['associatedAccounts'][number]
>;

interface BankAccountOverviewProps {
  accounts: AssociatedAccount[];
}

export function BankAccountOverview({ accounts }: BankAccountOverviewProps) {
  const institution = accounts[0]?.associatedInstitution;

  return (
    <Stack gap="sm" w="100%">
      {institution && (
        <Group className={classes.bankHeader} gap="md" wrap="nowrap">
          <Avatar
            src={institution.logoUri ? String(institution.logoUri) : undefined}
            radius="sm"
            size="lg"
            flex="0 0 auto"
          >
            {getInitials(institution.name)}
          </Avatar>
          <Stack gap={2} style={{ minWidth: 0 }}>
            <Text fw={600} size="md" truncate>
              {institution.name}
            </Text>
            <Text size="sm" c="dimmed">
              {institution.bic}
            </Text>
          </Stack>
        </Group>
      )}

      {accounts.map((account) => {
        const hasNameOrDescription = account.name || account.description;
        const balanceValue =
          account.balance != null
            ? account.currency
              ? `${String(account.balance)} ${account.currency}`
              : String(account.balance)
            : null;

        return (
          <div key={String(account.id)} className={classes.accountCard}>
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
      })}
    </Stack>
  );
}
