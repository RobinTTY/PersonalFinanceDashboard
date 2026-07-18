import { useCallback, useState } from 'react';
import { Avatar, Group, Stack, Text } from '@mantine/core';
import { getInitials } from '@/utility/getInitials';
import { BankAccountCard, BankAccountInstitution } from './BankAccountCard';
import classes from './BankAccountOverview.module.css';

interface BankAccountOverviewProps {
  /** Internal ids of the accounts associated with the completed authentication request. */
  accountIds: string[];
}

/**
 * Renders the accounts linked by a completed authentication request.
 *
 * Each account is loaded individually by id (see {@link BankAccountCard}) so its details are synced
 * on demand. The institution header is shared across accounts, so it's populated from the first
 * account that finishes loading.
 */
export function BankAccountOverview({ accountIds }: BankAccountOverviewProps) {
  const [institution, setInstitution] = useState<BankAccountInstitution | undefined>(undefined);

  const handleInstitutionLoaded = useCallback((loaded: BankAccountInstitution) => {
    setInstitution((current) => current ?? loaded);
  }, []);

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

      {accountIds.map((accountId) => (
        <BankAccountCard
          key={accountId}
          accountId={accountId}
          onInstitutionLoaded={handleInstitutionLoaded}
        />
      ))}
    </Stack>
  );
}
