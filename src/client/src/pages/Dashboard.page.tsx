import { useMemo } from 'react';
import { useQuery } from '@apollo/client/react';
import { GetNetWorthHistory } from '@graphql-queries/GetNetWorthHistory';
import { IconAlertCircle } from '@tabler/icons-react';
import { Alert, Card, Center, Loader, Stack, Text } from '@mantine/core';
import {
  buildNetWorthHistory,
  NetWorthAccount,
} from '@/components/NetWorthLineChart/utility/buildNetWorthHistory';
import { NetWorthLabel, NetWorthPanel } from '@/components/NetWorthPanel/NetWorthPanel';
import { SyncStatusAlert } from '@/components/SyncStatusIndicator/SyncStatusAlert';

const CHART_HEIGHT = 320;

export function DashboardPage() {
  const { data, loading, error } = useQuery(GetNetWorthHistory, {
    context: { notifyOnError: false },
  });

  const accounts = useMemo<NetWorthAccount[]>(
    () =>
      (data?.bankAccounts?.nodes ?? []).map((account) => ({
        balance: account.balance,
        currency: account.currency,
        transactions: account.transactions,
      })),
    [data]
  );

  const { dataByLabel, currency } = useMemo(() => buildNetWorthHistory(accounts), [accounts]);
  const syncStates = (data?.bankAccounts?.nodes ?? []).map((account) => account.syncStatus.state);

  const hasData = accounts.length > 0 && !loading && !error;

  return (
    <Stack>
      <SyncStatusAlert states={syncStates} />
      <Card withBorder radius="md" padding="lg">
        {hasData ? (
          <NetWorthPanel dataByLabel={dataByLabel} height={CHART_HEIGHT} currency={currency} />
        ) : (
          <Stack gap="lg">
            <NetWorthLabel />
            {error ? (
              <Alert
                color="red"
                icon={<IconAlertCircle size={16} />}
                title="Failed to load net worth"
              >
                {error.message}
              </Alert>
            ) : loading ? (
              <Center h={CHART_HEIGHT}>
                <Loader size="sm" />
              </Center>
            ) : (
              <Center h={CHART_HEIGHT}>
                <Text size="sm" c="dimmed">
                  Connect a bank account to see your net worth over time.
                </Text>
              </Center>
            )}
          </Stack>
        )}
      </Card>
    </Stack>
  );
}
