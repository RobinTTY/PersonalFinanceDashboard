import { Loader, Center } from "@mantine/core";
import { gql, useQuery } from "@apollo/client";
import { StatCardProps } from "../components/stat-card/StatCardProps";
import StatsGrid from "../components/stat-grid/StatsGrid";

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

  const gridprops: StatCardProps[] = [
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
