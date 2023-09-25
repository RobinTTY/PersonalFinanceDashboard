import { useQuery } from '@apollo/client';
import { Center, Loader } from '@mantine/core';
import { GetTransactionsQuery } from '@graphql-queries/GetTransactions';

export const AccountImportStep = () => {
  const { loading, data } = useQuery(GetTransactionsQuery, {
    variables: {
      accountId: '072fefa4-4530-4322-aafe-e953d37402ae',
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

  console.log(data);

  return <div>AccountImportStep</div>;
};
