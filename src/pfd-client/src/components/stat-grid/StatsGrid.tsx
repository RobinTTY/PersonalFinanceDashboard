// TODO: move to extra file
import { SimpleGrid } from '@mantine/core';
import { StatsGridProps } from './StatsGridProps';
import { StatCard } from '../stat-card/StatCard';

// TODO: should use children
export const StatsGrid = (props: StatsGridProps) => {
  return (
    <SimpleGrid cols={{ xs: 1, md: 2, lg: 4 }}>
      {props.data.map((stat) => {
        return <StatCard key={stat.title} {...stat}></StatCard>;
      })}
    </SimpleGrid>
  );
};
