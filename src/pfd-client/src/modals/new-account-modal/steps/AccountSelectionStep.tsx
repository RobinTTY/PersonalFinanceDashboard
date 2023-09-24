import { useQuery } from '@apollo/client';
import { Center, Loader, Text } from '@mantine/core';
import { Authentication } from './AuthenticationStep';
import { GetAccountQuery } from '@/graphql/queries/GetAccount';

export const AccountSelectionStep = ({ authentication }: AccountSelectionStepProps) => {
  const { loading, error, data } = useQuery(GetAccountQuery, {
    variables: {
      accountId: '072fefa4-4530-4322-aafe-e953d37402ae',
    },
  });

  // TODO: Create reusable loader component
  if (loading)
    return (
      <Center h={'100%'}>
        <Loader color="violet" />
      </Center>
    );

  console.log(authentication);
  return <Text>Account Selection</Text>;
};

export interface AccountSelectionStepProps {
  authentication: Authentication;
}
