import { useMemo } from 'react';
import { useQuery } from '@apollo/client/react';
import { GetNetWorthHistory } from '@graphql-queries/GetNetWorthHistory';
import { IconAlertCircle } from '@tabler/icons-react';
import { Alert, Card, Center, Loader, Stack, Text, Title } from '@mantine/core';
import { NetWorthLineChart } from '@/components/NetWorthLineChart/NetWorthLineChart';
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

  const subtitle = currency
    ? `Total accounts balance over time (${currency})`
    : 'Total accounts balance over time';

  return (
    <Stack>
      <Title order={2}>Dashboard</Title>

      <Card withBorder>
        <Stack gap="xs">
          <Text fw={600}>Net Worth</Text>
          <Text size="sm" c="dimmed">
            {subtitle}
          </Text>

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
          ) : accounts.length === 0 ? (
            <Center h={CHART_HEIGHT}>
              <Text size="sm" c="dimmed">
                Connect a bank account to see your net worth over time.
              </Text>
            </Center>
          ) : (
            <NetWorthLineChart dataByLabel={dataByLabel} height={CHART_HEIGHT} />
          )}
        </Stack>
      </Card>
    </Stack>
  );
}
