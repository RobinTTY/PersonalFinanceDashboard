import { Box, Space } from '@mantine/core';
import { StatCardProps } from '@components/stat-card/StatCardProps';
import { LineGraph } from '@components/graphs/line-graph/LineGraph';
import { StatsGrid } from '@components/stat-grid/StatsGrid';
import { SampleLineData } from '@/components/graphs/line-graph/SampleLineData';

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
      <LineGraph option={SampleLineData} />
      <Space h="md" />
      <StatsGrid data={gridprops} />
    </Box>
  );
};
