import { Paper, Text } from "@mantine/core";
import { AccountSummaryProps } from "./AccountSummaryProps";

export const AccountSummary = (props: AccountSummaryProps) => {
  // TODO: make formatter dynamic
  const formatter = new Intl.NumberFormat("en-US", {
    style: "currency",
    currency: "USD",

    // These options are needed to round to whole numbers if that's what you want.
    //minimumFractionDigits: 0, // (this suffices for whole numbers, but will print 2500.10 as $2,500.1)
    //maximumFractionDigits: 0, // (causes 2500.99 to be printed as $2,501)
  });

  return (
    <Paper withBorder p="md" radius="md">
      <Text size="xs" color="dimmed">
        {props.description}
      </Text>
      {/* TODO: text can't be white only => light mode */}
      <Text size="lg" color="white">
        {formatter.format(props.balance)}
      </Text>
    </Paper>
  );
};
