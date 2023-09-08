import { createStyles, Group, Paper, Text, rem } from "@mantine/core";
import { IconArrowUpRight, IconArrowDownRight } from "@tabler/icons-react";
import { StatCardProps, icons } from "./StatCardProps";

const useStyles = createStyles((theme) => ({
  value: {
    fontSize: rem(24),
    fontWeight: 700,
    lineHeight: 1,
  },

  diff: {
    lineHeight: 1,
    display: "flex",
    alignItems: "center",
  },

  icon: {
    color:
      theme.colorScheme === "dark"
        ? theme.colors.dark[3]
        : theme.colors.gray[4],
  },

  title: {
    fontWeight: 700,
    textTransform: "uppercase",
  },
}));

export const StatCard = (props: StatCardProps) => {
  const { classes } = useStyles();
  const Icon = icons[props.icon];
  const DiffIcon = props.diff > 0 ? IconArrowUpRight : IconArrowDownRight;

  return (
    <Paper withBorder p="md" radius="md">
      <Group position="apart">
        <Text size="xs" color="dimmed" className={classes.title}>
          {props.title}
        </Text>
        <Icon className={classes.icon} size="1.4rem" stroke={1.5} />
      </Group>

      <Group align="flex-end" spacing="xs" mt={25}>
        <Text className={classes.value}>{props.value}</Text>
        <Text
          color={props.diff > 0 ? "teal" : "red"}
          fz="sm"
          fw={500}
          className={classes.diff}
        >
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
