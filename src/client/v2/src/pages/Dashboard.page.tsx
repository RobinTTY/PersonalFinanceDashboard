import { Card, Stack, Text, Title } from '@mantine/core';
import { NetWorthLineChart } from '@/components/NetWorthLineChart/NetWorthLineChart';

export function DashboardPage() {
  const accountBalanceByMonth = {
    Oct: 12400,
    Nov: 12950,
    Dec: 13100,
    Jan: 13820,
    Feb: 14210,
    Mar: 14940,
  };

  return (
    <Stack>
      <Title order={2}>Dashboard</Title>

      <Card withBorder>
        <Stack gap="xs">
          <Text fw={600}>Net Worth</Text>
          <Text size="sm" c="dimmed">
            Total Accounts Balance Over Time
          </Text>
          <NetWorthLineChart dataByLabel={accountBalanceByMonth} />
        </Stack>
      </Card>
    </Stack>
  );
}
