import { Loader, Center } from "@mantine/core";
import StatsGrid, { StatsGridProps } from "../components/stat-card/StatCard";
import { gql, useQuery } from "@apollo/client";

const query = gql`
  query GetAccounts {
    accounts {
      description
      balance
      currency
      type
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
  data.accounts.forEach((account: any) => {
    console.log(account);
  });

  const gridprops: StatsGridProps["data"] = [
    { title: "Revenue", icon: "receipt", value: "13,456", diff: 34 },
    { title: "Profit", icon: "coin", value: "4,145", diff: -13 },
    { title: "Coupons usage", icon: "discount", value: "745", diff: 18 },
    { title: "New customers", icon: "user", value: "188", diff: -30 },
  ];

  return (
    <div>
      <StatsGrid data={gridprops}></StatsGrid>
    </div>
  );
};

export default Dashboard;
