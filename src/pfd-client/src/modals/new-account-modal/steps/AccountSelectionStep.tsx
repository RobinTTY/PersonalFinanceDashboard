import { useQuery } from '@apollo/client';
import { Center, Loader, Text } from '@mantine/core';
import { Authentication } from './AuthenticationStep';
import { GetAccountsQuery } from '@/graphql/queries/GetAccounts';

export const AccountSelectionStep = ({ authentication }: AccountSelectionStepProps) => {
  const { loading, data } = useQuery(GetAccountsQuery, {
    variables: {
      accountIds: authentication.associatedAccounts,
    },
  });

  // TODO: Create reusable loader component
  if (loading) {
    return (
      <Center h="100%">
        <Loader color="violet" />
      </Center>
    );
  }

  // TODO: safeguard
  return (
    <>
      {data?.accounts?.edges?.map((account) => {
        console.log(account);
        return <Text key={account.node.id}>Account</Text>;
      })}
    </>
  );
};

export interface AccountSelectionStepProps {
  authentication: Authentication;
}
