import { useQuery } from '@apollo/client';
import { Box, Center, Loader, Text, Button, Stack } from '@mantine/core';
import { IconCheck } from '@tabler/icons-react';
import { GetTransactionsQuery } from '@graphql-queries/GetTransactions';
import { AccountImportStepProps } from './AccountImportStepProps';

export const AccountImportStep = ({ onImportComplete }: AccountImportStepProps) => {
  const { loading, data } = useQuery(GetTransactionsQuery, {
    variables: {
      accountId: '072fefa4-4530-4322-aafe-e953d37402ae',
      first: 15,
    },
  });

  // TODO: Create reusable loader component
  if (loading) {
    return (
      <Box p="md">
        <Center>
          <Loader color="violet" />
        </Center>
        <Text fw="bold" pt="md" w={200} ta="center">
          Your accounts are being imported...
        </Text>
      </Box>
    );
  }

  console.log(data);

  return (
    <Stack p="md" pb={0} gap="sm">
      <Center>
        <IconCheck size={48} color="var(--mantine-color-green-filled)" stroke={3.5} />
      </Center>
      <Center>
        <Text fw="bold" w={200} ta="center">
          Your accounts were successfully imported!
        </Text>
      </Center>
      <Center pt="xs">
        <Button size="md" onClick={onImportComplete}>
          Close
        </Button>
      </Center>
    </Stack>
  );
};
