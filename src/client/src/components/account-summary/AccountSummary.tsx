import { Paper, Text } from '@mantine/core';
import { AccountSummaryProps } from './AccountSummaryProps';
import { getCurrencyFormatter } from '@/utility/getCurrencyFormatter';

export const AccountSummary = (props: AccountSummaryProps) => {
  // TODO: make formatter dynamic
  const formatter = getCurrencyFormatter('en-US', 'USD');

  return (
    <Paper withBorder p="md" radius="md">
      <Text size="xs" c="dimmed">
        {props.description}
      </Text>
      {/* TODO: text can't be white only => light mode */}
      <Text size="lg" c="white">
        {formatter.format(props.balance)}
      </Text>
    </Paper>
  );
};
