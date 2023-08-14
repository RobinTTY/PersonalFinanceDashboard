import { useQuery } from "@apollo/client";
import { GetAccountsQuery } from "../queries/GetAccounts";
import { Center, Loader, SimpleGrid } from "@mantine/core";
import AccountSummary from "../components/account-summary/AccountSummary";
import { AccountSummaryProps } from "../components/account-summary/AccountSummaryProps";

// TODO: Add all accounts view?
const Accounts = () => {
  const { loading, error, data } = useQuery(GetAccountsQuery, {
    variables: { first: 5 },
  });

  let accounts: any[] = [];
  if (data)
    data.accounts.edges.map((account: any) => {
      accounts.push(account.node);
      console.log(account.node);
    });

  if (loading)
    return (
      <Center h={"100%"}>
        <Loader color="violet" />
      </Center>
    );

  return (
    <SimpleGrid cols={2} breakpoints={[{ maxWidth: "xs", cols: 1 }]}>
      {accounts.map((account: AccountSummaryProps) => {
        return <AccountSummary {...account} />;
      })}
    </SimpleGrid>
  );
};

export default Accounts;
