// TODO: move to extra file
import { SimpleGrid } from '@mantine/core';
import { StatCard } from '@components/stat-card/StatCard';
import { StatsGridProps } from './StatsGridProps';

// TODO: should use children
export const StatsGrid = (props: StatsGridProps) => (
  <SimpleGrid cols={{ xs: 1, md: 2, lg: 4 }}>
    {props.data.map((stat) => (
      <StatCard key={stat.title} {...stat} />
    ))}
  </SimpleGrid>
);
