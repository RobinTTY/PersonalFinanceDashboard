import { Box, Space } from '@mantine/core';

import { StatCardProps } from '@components/stat-card/StatCardProps';
import { LineGraph } from '@components/graphs/line-graph/LineGraph';
import { StatsGrid } from '@components/stat-grid/StatsGrid';

export const Dashboard = () => {
  const gridprops: StatCardProps[] = [
    { title: 'Net Worth', icon: 'coin', value: '$13,456', diff: 34 },
    { title: 'Profit', icon: 'receipt', value: '$145.56', diff: -13 },
    { title: 'Transactions', icon: 'discount', value: '23', diff: 8 },
    {
      title: 'Followers',
      icon: 'user',
      value: '1835',
      diff: 3,
    },
  ];

  return (
    <Box p="md" style={{ height: '100%' }}>
      <LineGraph />
      <Space h="md" />
      <StatsGrid data={gridprops} />
    </Box>
  );
};
