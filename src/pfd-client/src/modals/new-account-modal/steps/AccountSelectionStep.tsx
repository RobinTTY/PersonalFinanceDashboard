import { QueryResult, useQuery } from '@apollo/client';
import { Center, Loader, Text } from '@mantine/core';
import { Authentication } from './AuthenticationStep';
import { GetAccountsQuery } from '@/graphql/queries/GetAccounts';

export const AccountSelectionStep = ({ authentication }: AccountSelectionStepProps) => {
  const { loading, error, data } = useQuery(GetAccountsQuery, {
    variables: {
      accountIds: authentication.associatedAccounts,
    },
  });

  // TODO: Create reusable loader component
  if (loading)
    return (
      <Center h={'100%'}>
        <Loader color="violet" />
      </Center>
    );

  console.log(data);
  return <Text>Account Selection</Text>;
};

export interface AccountSelectionStepProps {
  authentication: Authentication;
}
