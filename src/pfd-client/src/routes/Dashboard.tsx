import { Loader, Center } from "@mantine/core";
import { gql, useQuery } from "@apollo/client";
import { StatCardProps } from "../components/stat-card/StatCardProps";
import StatsGrid from "../components/stat-grid/StatsGrid";

const query = gql`
  query GetAccounts {
    accounts(first: 10) {
      edges {
        node {
          description
          balance
          currency
          type
        }
      }
    }
  }
`;

const Dashboard = () => {
  const { loading, error, data } = useQuery(query);
  if (loading)
    return (
      <Center h={"100%"}>
        <Loader color="violet" />
      </Center>
    );

  console.log(data.accounts.edges);

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
    <div>
      <StatsGrid data={gridprops}></StatsGrid>
    </div>
  );
};

export default Dashboard;
