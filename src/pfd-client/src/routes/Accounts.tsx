import { useQuery } from "@apollo/client";
import { GetAccountsQuery } from "../queries/GetAccounts";
import { Center, Loader, SimpleGrid } from "@mantine/core";
import AccountSummary from "../components/account-summary/AccountSummary";
import { AccountSummaryProps } from "../components/account-summary/AccountSummaryProps";
import LineGraph from "../components/graphs/line-graph/LineGraph";

// TODO: Add all accounts view?
// TODO: Add icon (e.g. bank logo)
const Accounts = () => {
  const { loading, error, data } = useQuery(GetAccountsQuery, {
    variables: { first: 5 },
  });

  let accounts: any[] = [];
  if (data)
    data.accounts.edges.map((account: any) => {
      accounts.push(account.node);
      // console.log(account.node);
    });

  if (loading)
    return (
      <Center h={"100%"}>
        <Loader color="violet" />
      </Center>
    );

  return (
    <>
      <LineGraph />
      <SimpleGrid mt="2%" cols={2} breakpoints={[{ maxWidth: "xs", cols: 1 }]}>
        {accounts.map((account: AccountSummaryProps) => {
          // TODO: Add key
          return <AccountSummary {...account} key={account.balance} />;
        })}
      </SimpleGrid>
    </>
  );
};

export default Accounts;
