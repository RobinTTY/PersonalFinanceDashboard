// TODO: move to extra file
import { createStyles, SimpleGrid } from "@mantine/core";
import { StatsGridProps } from "./StatsGridProps";
import StatCard from "../stat-card/StatCard";

const useStyles = createStyles((theme) => ({
  root: {
    padding: `calc(${theme.spacing.xl} * 1.5)`,
  },
}));

// TODO: should use children
const StatsGrid = (props: StatsGridProps) => {
  const { classes } = useStyles();

  return (
    <div className={classes.root}>
      <SimpleGrid
        cols={4}
        breakpoints={[
          { maxWidth: "md", cols: 2 },
          { maxWidth: "xs", cols: 1 },
        ]}
      >
        {props.data.map((stat) => {
          return <StatCard key={stat.title} {...stat}></StatCard>;
        })}
      </SimpleGrid>
    </div>
  );
};

export default StatsGrid;
