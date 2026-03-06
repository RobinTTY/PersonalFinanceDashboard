import { Card, Stack, Text, Title } from '@mantine/core';
import { NetWorthLineChart } from '@/components/NetWorthLineChart/NetWorthLineChart';

export function DashboardPage() {
  const accountBalanceData = [12400, 12950, 13100, 13820, 14210, 14940];

  return (
    <Stack>
      <Title order={2}>Dashboard</Title>

      <Card withBorder>
        <Stack gap="xs">
          <Text fw={600}>Net Worth</Text>
          <Text size="sm" c="dimmed">
            Total Accounts Balance Over Time
          </Text>
          <NetWorthLineChart
            data={accountBalanceData}
            labels={['Oct', 'Nov', 'Dec', 'Jan', 'Feb', 'Mar']}
          />
        </Stack>
      </Card>
    </Stack>
  );
}
