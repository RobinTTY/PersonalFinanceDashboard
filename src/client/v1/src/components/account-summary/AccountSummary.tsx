import { Box, Center, Group, Paper, Text } from '@mantine/core';
import { AccountSummaryProps } from './AccountSummaryProps';
import { getCurrencyFormatter } from '@/utility/getCurrencyFormatter';

import classes from './AccountSummary.module.css';

export const AccountSummary = ({ description, balance, currency, icon }: AccountSummaryProps) => {
  // TODO: make formatter dynamic
  const formatter = getCurrencyFormatter('en-US', currency);

  const textContent = (
    <Box>
      <Text size="xs" c="dimmed">
        {description}
      </Text>
      {/* TODO: text can't be white only => light mode */}
      <Text size="lg" className={classes.balance}>
        {formatter.format(balance)}
      </Text>
    </Box>
  );

  const iconAndTextContent = (
    <Group>
      <Center className={classes.icon}>{icon}</Center>
      {textContent}
    </Group>
  );

  const totalContent = icon ? iconAndTextContent : textContent;

  return (
    <Paper withBorder p="md" radius="md">
      {totalContent}
    </Paper>
  );
};
