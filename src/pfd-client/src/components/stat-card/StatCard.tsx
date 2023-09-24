import { Group, Paper, Text, rem } from '@mantine/core';
import { IconArrowUpRight, IconArrowDownRight } from '@tabler/icons-react';
import { StatCardProps, icons } from './StatCardProps';

import classes from './StatCard.module.css';

export const StatCard = (props: StatCardProps) => {
  const Icon = icons[props.icon];
  const DiffIcon = props.diff > 0 ? IconArrowUpRight : IconArrowDownRight;

  return (
    <Paper withBorder p="md" radius="md">
      <Group justify="apart">
        <Text size="xs" c="dimmed" className={classes.title}>
          {props.title}
        </Text>
        <Icon className={classes.icon} size="1.4rem" stroke={1.5} />
      </Group>

      <Group align="flex-end" gap="xs" mt={25}>
        <Text className={classes.value}>{props.value}</Text>
        <Text c={props.diff > 0 ? 'teal' : 'red'} fz="sm" fw={500} className={classes.diff}>
          <span>{props.diff}%</span>
          <DiffIcon size="1rem" stroke={1.5} />
        </Text>
      </Group>

      <Text fz="xs" c="dimmed" mt={7}>
        Compared to previous month
      </Text>
    </Paper>
  );
};
