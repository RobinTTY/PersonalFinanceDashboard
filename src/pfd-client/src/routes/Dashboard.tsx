import { Loader, Center } from "@mantine/core";
import { useQuery } from "@apollo/client";

import { StatCardProps } from "../components/stat-card/StatCardProps";
import { GetAccountsQuery } from "../queries/GetAccounts";
import { LineGraph } from "../components/graphs/line-graph/LineGraph";
import { StatsGrid } from "../components/stat-grid/StatsGrid";

export const Dashboard = () => {
  const { loading, error, data } = useQuery(GetAccountsQuery, {
    variables: { first: 10 },
  });

  // TODO: Remove this duplicate code (also used in Accounts)
  if (loading)
    return (
      <Center h={"100%"}>
        <Loader color="violet" />
      </Center>
    );

  const gridprops: StatCardProps[] = [
    { title: "Net Worth", icon: "coin", value: "$13,456", diff: 34 },
    { title: "Profit", icon: "receipt", value: "$145.56", diff: -13 },
    { title: "Transactions", icon: "discount", value: "23", diff: 8 },
    {
      title: "Followers",
      icon: "user",
      value: "1835",
      diff: 3,
    },
  ];

  return (
    <>
      <LineGraph />
      <StatsGrid data={gridprops}></StatsGrid>
    </>
  );
};
