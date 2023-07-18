import StatsGrid, {
  StatsGridProps,
  icons,
} from "../components/stat-card/StatCard";

const Dashboard = () => {
  const data: StatsGridProps["data"] = [
    { title: "Revenue", icon: "receipt", value: "13,456", diff: 34 },
    { title: "Profit", icon: "coin", value: "4,145", diff: -13 },
    { title: "Coupons usage", icon: "discount", value: "745", diff: 18 },
    { title: "New customers", icon: "user", value: "188", diff: -30 },
  ];

  return (
    <div>
      <StatsGrid data={data}></StatsGrid>
    </div>
  );
};

export default Dashboard;
