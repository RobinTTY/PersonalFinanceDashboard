import { useMemo } from 'react';
import { useQuery } from '@apollo/client/react';
import { GetNetWorthHistory } from '@graphql-queries/GetNetWorthHistory';
import { IconAlertCircle } from '@tabler/icons-react';
import { Alert, Card, Center, Loader, Stack, Text, Title } from '@mantine/core';
import { NetWorthLabel, NetWorthPanel } from '@/components/NetWorthPanel/NetWorthPanel';
import {
  buildNetWorthHistory,
  NetWorthAccount,
} from '@/components/NetWorthLineChart/utility/buildNetWorthHistory';

const CHART_HEIGHT = 320;

export function DashboardPage() {
  const { data, loading, error } = useQuery(GetNetWorthHistory);

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

  const hasData = accounts.length > 0 && !loading && !error;

  return (
    <Stack>
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
